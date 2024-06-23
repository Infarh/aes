namespace FileEncryptor;

internal static class AesEx
{
    public static CryptoStream GetEncryptionStream(this Aes aes, Stream src) => new(src, aes.CreateEncryptor(), CryptoStreamMode.Write);

    public static CryptoStream GetDecryptionStream(this Aes aes, Stream src) => new(src, aes.CreateDecryptor(), CryptoStreamMode.Read);
}
