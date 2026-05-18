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
    /// <param name="sourceFile">Исходный файл для шифрования</param>
    /// <param name="aes">Экземпляр алгоритма AES</param>
    public static void Encrypt(this FileInfo sourceFile, Aes aes)
    {
        var encryptedFile = new FileInfo($"{sourceFile.FullName}{Constants.EncodedExt}");

        Console.WriteLine($"Encrypting: {CheckRelatedPath(sourceFile.FullName)} {sourceFile.Length}B");
        Console.WriteLine($"        to: {CheckRelatedPath(encryptedFile.FullName)}");

        using var srcStream = sourceFile.OpenRead();
        using var dstStream = encryptedFile.Create();
        using var cryptStream = aes.GetEncryptionStream(dstStream);

        srcStream.CopyToStream(cryptStream, sourceFile.Length);

        Console.WriteLine("Encrypted.");
    }

    /// <summary>Расшифровывает файл с расширением .aes в исходный файл</summary>
    /// <param name="encryptedFile">Зашифрованный файл</param>
    /// <param name="aes">Экземпляр алгоритма AES</param>
    public static void Decrypt(this FileInfo encryptedFile, Aes aes)
    {
        var sourceFileFullName = Path.GetFileNameWithoutExtension(encryptedFile.FullName);
        var destFile = new FileInfo(sourceFileFullName);

        Console.WriteLine($"Decrypting: {CheckRelatedPath(encryptedFile.FullName)}");
        Console.WriteLine($"        to: {CheckRelatedPath(destFile.FullName)}");

        try
        {
            using var srcStream = encryptedFile.OpenRead();
            using var dstStream = destFile.Create();
            using var cryptStream = aes.GetDecryptionStream(srcStream);

            cryptStream.CopyToStream(dstStream, encryptedFile.Length);

            Console.WriteLine("Decrypted.");
        }
        catch (CryptographicException)
        {
            destFile.Delete();
            Console.WriteLine();
            Console.WriteLine("File name error");
        }
    }
}
