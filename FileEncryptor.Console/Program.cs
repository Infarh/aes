using FileEncryptor;

if (args.Length == 0)
{
    Console.WriteLine("Не указан файл для обработки");
    return;
}

const string encoded_ext = ".aes";

foreach (var file in args.Select(file_name => new FileInfo(file_name)).Where(f => f.Exists))
{
    var ext = file.Extension;

    if (string.Equals(ext, encoded_ext, StringComparison.OrdinalIgnoreCase))
        Decrypt(file);
    else
        Encrypt(file);
}

const int buffer_size = 1024 * 1024;

static void Encrypt(FileInfo file)
{
    Console.WriteLine("Шифрую файл {0}", file);

    var encoded_file = new FileInfo($"{file.FullName}{encoded_ext}");

    var file_name = Path.GetFileNameWithoutExtension(file.Name);

    using var aes = CreateAES(file_name);

    var encryptor = aes.CreateEncryptor();

    using var src_file = file.OpenRead();
    using var dst_file = encoded_file.Create();
    using var crypt_stream = new CryptoStream(dst_file, encryptor, CryptoStreamMode.Write);

    CopyToCryptoStream(src_file, crypt_stream);
}

static void Decrypt(FileInfo file)
{
    Console.WriteLine("Дешифрую файл {0}", file);

    var source_file_full_name = Path.GetFileNameWithoutExtension(file.FullName);
    var source_file = new FileInfo(source_file_full_name);

    var password = Path.GetFileNameWithoutExtension(source_file_full_name);
    using var aes = CreateAES(password);
    var decryptor = aes.CreateDecryptor();

    try
    {
        using var src_file = file.OpenRead();
        using var dst_file = source_file.Create();
        using var crypt_stream = new CryptoStream(dst_file, decryptor, CryptoStreamMode.Write);

        CopyToCryptoStream(src_file, crypt_stream);
    }
    catch (CryptographicException)
    {
        source_file.Delete();
        Console.CursorLeft = 0;
        Console.WriteLine("Ошибка в имени файла");
        Console.WriteLine();
    }
}

static void CopyToCryptoStream(FileStream Source, CryptoStream Destination)
{
    var buffer = new byte[buffer_size];
    var readed_total = 0L;
    double total_length = Source.Length;
    int readed;
    do
    {
        readed = Source.Read(buffer);
        Destination.Write(buffer, 0, readed);

        readed_total += readed;
        Console.CursorLeft = 0;
        Console.Write("Завершено {0:p2}", readed_total / total_length);
    }
    while (readed == buffer_size);

    Destination.FlushFinalBlock();

    Console.CursorLeft = 0;
    Console.WriteLine("Завершено.");
    Console.WriteLine();
}

static Aes CreateAES(string Pass)
{
    var pdb = new Rfc2898DeriveBytes(Pass, Constants.Salt);
    var aes = Aes.Create();
    aes.Key = pdb.GetBytes(32);
    aes.IV = pdb.GetBytes(16);
    return aes;
}

