namespace FileEncryptor;

/// <summary>Содержит константы и общие значения приложения</summary>
internal static class Constants
{
    /// <summary>Определяет расширение зашифрованного файла</summary>
    public const string EncodedExt = ".aes";

    /// <summary>Формирует соль фиксированной длины для вывода ключа</summary>
    /// <returns>Массив байт соли длиной 16</returns>
    private static byte[] GetSalt()
    {
        var assemblyName = Path.GetFileNameWithoutExtension(Environment.ProcessPath) ?? string.Empty;
        var assemblyNameBytes = Encoding.UTF8.GetBytes(assemblyName);
        byte[] salt = [
            .. assemblyNameBytes,
            0x26, 0xdc, 0xff, 0x00,
            0xad, 0xed, 0x7a, 0xee,
            0xc5, 0xfe, 0x07, 0xaf,
            0x4d, 0x08, 0x22, 0x3c,
        ];
        return salt[..16];
    }

    /// <summary>Хранит соль для алгоритма PBKDF2</summary>
    public static readonly byte[] Salt = GetSalt();
    //[
    //    0x26, 0xdc, 0xff, 0x00,
    //    0xad, 0xed, 0x7a, 0xee,
    //    0xc5, 0xfe, 0x07, 0xaf,
    //    0x4d, 0x08, 0x22, 0x3c,
    //];

    /// <summary>Возвращает базовый путь запуска приложения</summary>
    public static string CurrentPath { get; } = AppContext.BaseDirectory;
}