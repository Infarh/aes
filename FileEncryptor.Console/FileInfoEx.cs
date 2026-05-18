using System.Buffers;

namespace FileEncryptor;

/// <summary>Содержит расширения для шифрования и расшифровки файлов</summary>
internal static class FileInfoEx
{
    /// <summary>Преобразует абсолютный путь в относительный к каталогу приложения</summary>
    /// <param name="path">Исходный путь файла</param>
    /// <returns>Относительный или исходный путь</returns>
    private static string CheckRelatedPath(string path) =>
        path.StartsWith(Constants.CurrentPath, StringComparison.OrdinalIgnoreCase)
            ? path[Constants.CurrentPath.Length..].TrimStart('/', '\\')
            : path;

    /// <summary>Шифрует файл и сохраняет результат с расширением .aes</summary>
    /// <param name="SourceFile">Исходный файл для шифрования</param>
    /// <param name="Password">Строка для вывода ключей</param>
    public static void Encrypt(this FileInfo SourceFile, string Password)
    {
        ArgumentNullException.ThrowIfNull(SourceFile);
        ArgumentException.ThrowIfNullOrWhiteSpace(Password);

        var encrypted_file = new FileInfo($"{SourceFile.FullName}{Constants.EncodedExt}");
        var salt = RandomNumberGenerator.GetBytes(Constants.SaltSize);
        using var aes = CreateAes(Password, salt, out var macKey);

        Console.WriteLine($"Encrypting: {CheckRelatedPath(SourceFile.FullName)} {SourceFile.Length}B");
        Console.WriteLine($"        to: {CheckRelatedPath(encrypted_file.FullName)}");

        using var src_stream = SourceFile.OpenRead();
        using var dst_stream = encrypted_file.Create();
        WriteHeader(dst_stream, salt);
        using var crypt_stream = aes.GetEncryptionStream(dst_stream);

        src_stream.CopyToStream(crypt_stream, SourceFile.Length);

        encrypted_file.Refresh();
        var signed_length = encrypted_file.Length;
        using var signed_stream = encrypted_file.OpenRead();
        var tag = ComputeHmacPrefix(signed_stream, macKey, signed_length);

        using var tag_stream = encrypted_file.Open(FileMode.Append, FileAccess.Write, FileShare.None);
        tag_stream.Write(tag);

        Console.WriteLine("Encrypted.");
    }

    /// <summary>Расшифровывает файл с расширением .aes в исходный файл</summary>
    /// <param name="EncryptedFile">Зашифрованный файл</param>
    /// <param name="Password">Строка для вывода ключей</param>
    public static void Decrypt(this FileInfo EncryptedFile, string Password)
    {
        ArgumentNullException.ThrowIfNull(EncryptedFile);
        ArgumentException.ThrowIfNullOrWhiteSpace(Password);

        var source_file_full_name = Path.GetFileNameWithoutExtension(EncryptedFile.FullName);
        var dest_file = new FileInfo(source_file_full_name);

        Console.WriteLine($"Decrypting: {CheckRelatedPath(EncryptedFile.FullName)}");
        Console.WriteLine($"        to: {CheckRelatedPath(dest_file.FullName)}");

        using var src_stream = EncryptedFile.OpenRead();
        if (src_stream.Length < Constants.HeaderSize + Constants.HmacSize)
            throw new InvalidDataException("Файл имеет неверный размер или поврежден");

        var salt = ReadAndValidateHeader(src_stream);
        using var aes = CreateAes(Password, salt, out var macKey);

        var signed_length = src_stream.Length - Constants.HmacSize;
        var expected_tag = ReadTagAt(src_stream, signed_length);
        var actual_tag = ComputeHmacPrefix(src_stream, macKey, signed_length);

        if (!CryptographicOperations.FixedTimeEquals(actual_tag, expected_tag))
            throw new CryptographicException("Не пройдена проверка целостности или пароль неверен");

        var encrypted_payload_length = signed_length - Constants.HeaderSize;
        src_stream.Position = Constants.HeaderSize;
        try
        {
            using var dst_stream = dest_file.Create();
            using var crypt_stream = aes.GetDecryptionWriteStream(dst_stream);
            src_stream.CopyToStream(crypt_stream, encrypted_payload_length);

            Console.WriteLine("Decrypted.");
        }
        catch
        {
            dest_file.Delete();
            throw;
        }
    }

    /// <summary>Создает и настраивает AES, а также выводит ключ для HMAC</summary>
    /// <param name="Password">Строка для вывода ключей</param>
    /// <param name="Salt">Соль PBKDF2</param>
    /// <param name="MacKey">Выходной ключ для HMAC-SHA256</param>
    /// <returns>Настроенный экземпляр AES</returns>
    private static Aes CreateAes(string Password, byte[] Salt, out byte[] MacKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Password);
        ArgumentNullException.ThrowIfNull(Salt);

        var aes = Aes.Create();
        var key_material_size = Constants.AesKeySize + Constants.AesIvSize + Constants.HmacSize;
        var key_material = Rfc2898DeriveBytes.Pbkdf2(Password, Salt, Constants.Pbkdf2Iterations, HashAlgorithmName.SHA512, key_material_size);

        aes.Key = key_material[..Constants.AesKeySize];
        aes.IV = key_material[Constants.AesKeySize..(Constants.AesKeySize + Constants.AesIvSize)];
        MacKey = key_material[(Constants.AesKeySize + Constants.AesIvSize)..];
        return aes;
    }

    /// <summary>Записывает заголовок формата в поток назначения</summary>
    /// <param name="Destination">Поток назначения</param>
    /// <param name="Salt">Соль PBKDF2</param>
    private static void WriteHeader(Stream Destination, byte[] Salt)
    {
        Destination.Write(Constants.FormatMagic);
        Destination.Write(Salt);
    }

    /// <summary>Читает и проверяет заголовок формата, затем возвращает соль</summary>
    /// <param name="source">Исходный поток</param>
    /// <returns>Соль PBKDF2</returns>
    /// <exception cref="InvalidDataException">Вызывается при несовместимом формате файла</exception>
    private static byte[] ReadAndValidateHeader(Stream source)
    {
        var magic = new byte[Constants.FormatMagic.Length];
        source.ReadExactly(magic);

        if (!magic.AsSpan().SequenceEqual(Constants.FormatMagic))
            throw new InvalidDataException("Неподдерживаемый формат зашифрованного файла");

        var salt = new byte[Constants.SaltSize];
        source.ReadExactly(salt);
        return salt;
    }

    /// <summary>Вычисляет HMAC для префикса потока заданной длины</summary>
    /// <param name="Source">Исходный поток</param>
    /// <param name="MacKey">Ключ HMAC</param>
    /// <param name="Length">Количество байт для хеширования от начала потока</param>
    /// <returns>Значение HMAC-SHA256</returns>
    private static byte[] ComputeHmacPrefix(Stream Source, byte[] MacKey, long Length)
    {
        ArgumentNullException.ThrowIfNull(Source);
        ArgumentNullException.ThrowIfNull(MacKey);

        Source.Position = 0;
        using var hmac = new HMACSHA256(MacKey);
        var remaining = Length;
        var buffer_array = ArrayPool<byte>.Shared.Rent(1024 * 1024);

        try
        {
            while (remaining > 0)
            {
                var read_size = (int)Math.Min(buffer_array.Length, remaining);
                var read_bytes = Source.Read(buffer_array, 0, read_size);
                if (read_bytes <= 0)
                    throw new EndOfStreamException("Недостаточно данных для проверки целостности");

                hmac.TransformBlock(buffer_array, 0, read_bytes, null, 0);
                remaining -= read_bytes;
            }

            hmac.TransformFinalBlock([], 0, 0);
            return hmac.Hash ?? throw new CryptographicException("Ошибка вычисления контрольной суммы");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer_array, clearArray: true);
        }
    }

    /// <summary>Считывает HMAC-тег по заданному смещению</summary>
    /// <param name="Source">Исходный поток</param>
    /// <param name="Offset">Смещение начала тега</param>
    /// <returns>Считанный HMAC-тег</returns>
    private static byte[] ReadTagAt(Stream Source, long Offset)
    {
        Source.Position = Offset;
        var tag = new byte[Constants.HmacSize];
        Source.ReadExactly(tag);
        return tag;
    }
}
