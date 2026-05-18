namespace FileEncryptor;

/// <summary>Содержит расширения для создания криптографических потоков AES</summary>
internal static class AesEx
{
    /// <summary>Создает поток шифрования для целевого потока</summary>
    /// <param name="aes">Экземпляр алгоритма AES</param>
    /// <param name="src">Целевой поток для записи зашифрованных данных</param>
    /// <returns>Криптографический поток в режиме записи</returns>
    public static CryptoStream GetEncryptionStream(this Aes aes, Stream src) => new(src, aes.CreateEncryptor(), CryptoStreamMode.Write);

    /// <summary>Создает поток расшифровки для исходного потока</summary>
    /// <param name="aes">Экземпляр алгоритма AES</param>
    /// <param name="src">Исходный поток с зашифрованными данными</param>
    /// <returns>Криптографический поток в режиме чтения</returns>
    public static CryptoStream GetDecryptionStream(this Aes aes, Stream src) => new(src, aes.CreateDecryptor(), CryptoStreamMode.Read);

    /// <summary>Создает поток расшифровки для записи открытых данных в целевой поток</summary>
    /// <param name="aes">Экземпляр алгоритма AES</param>
    /// <param name="src">Целевой поток для записи расшифрованных данных</param>
    /// <returns>Криптографический поток в режиме записи</returns>
    public static CryptoStream GetDecryptionWriteStream(this Aes aes, Stream src) => new(src, aes.CreateDecryptor(), CryptoStreamMode.Write);
}
