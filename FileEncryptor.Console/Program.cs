using FileEncryptor;

/// <summary>Определяет точку входа консольного приложения</summary>
internal static class Program
{
    /// <summary>Запускает обработку переданных файлов</summary>
    /// <param name="args">Список путей к файлам</param>
    /// <returns>Код завершения процесса</returns>
    private static int Main(string[] args)
    {
        if (args is [])
        {
            Console.WriteLine("No files to processing");
            return 0;
        }

        foreach (var file in args.Select(fileName => new FileInfo(fileName)).Where(f => f.Exists))
        {
            var ext = file.Extension;

            if (string.Equals(ext, Constants.EncodedExt, StringComparison.OrdinalIgnoreCase))
            {
                var password = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name));
                using var aes = CreateAes(password);
                file.Decrypt(aes);
            }
            else
            {
                var password = Path.GetFileNameWithoutExtension(file.Name);
                using var aes = CreateAes(password);
                file.Encrypt(aes);
            }
        }

        return 0;
    }

    /// <summary>Создает и инициализирует экземпляр AES на основе строки пароля</summary>
    /// <param name="pass">Строка для вывода ключа и вектора инициализации</param>
    /// <returns>Настроенный экземпляр AES</returns>
    private static Aes CreateAes(string pass)
    {
        var aes = Aes.Create();

        var key_and_iv = Rfc2898DeriveBytes.Pbkdf2(
            pass,
            Constants.Salt,
            13,
            HashAlgorithmName.SHA512,
            48);

        aes.Key = key_and_iv[..32];
        aes.IV = key_and_iv[32..];
        return aes;
    }
}