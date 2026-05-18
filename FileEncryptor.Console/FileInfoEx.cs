namespace FileEncryptor;

internal static class FileInfoEx
{
    private static string CheckRelatedPath(string path) =>
        path.StartsWith(Constants.CurrentPath, StringComparison.OrdinalIgnoreCase)
            ? path[Constants.CurrentPath.Length..].TrimStart('/', '\\')
            : path;

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
