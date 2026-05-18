namespace FileEncryptor;

/// <summary>Содержит константы и общие значения приложения</summary>
internal static class Constants
{
    /// <summary>Определяет расширение зашифрованного файла</summary>
    public const string EncodedExt = ".aes";

    /// <summary>Определяет размер соли для PBKDF2 в байтах</summary>
    public const int SaltSize = 16;

    /// <summary>Определяет размер HMAC-SHA256 тега в байтах</summary>
    public const int HmacSize = 32;

    /// <summary>Определяет длину ключа AES-256 в байтах</summary>
    public const int AesKeySize = 32;

    /// <summary>Определяет длину IV для AES-CBC в байтах</summary>
    public const int AesIvSize = 16;

    /// <summary>Определяет количество итераций PBKDF2</summary>
    public const int Pbkdf2Iterations = 210000;

    /// <summary>Содержит сигнатуру формата зашифрованного файла</summary>
    public static readonly byte[] FormatMagic = Encoding.ASCII.GetBytes("AES1");

    /// <summary>Возвращает размер заголовка зашифрованного файла в байтах</summary>
    public static int HeaderSize { get; } = FormatMagic.Length + SaltSize;

    /// <summary>Возвращает базовый путь запуска приложения</summary>
    public static string CurrentPath { get; } = AppContext.BaseDirectory;
}