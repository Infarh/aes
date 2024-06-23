namespace FileEncryptor;

internal static class FileInfoEx
{
    private static string CheckRelatedPath(string path) =>
        path.StartsWith(Constants.CurrentPath, StringComparison.OrdinalIgnoreCase)
            ? path[Constants.CurrentPath.Length..].TrimStart('/', '\\')
            : path;

    public static void Encrypt(this FileInfo SourceFile, Aes aes)
    {
        var encrypted_file = new FileInfo($"{SourceFile.FullName}{Constants.EncodedExt}");

        Console.WriteLine($"Encrypting: {CheckRelatedPath(SourceFile.FullName)} {SourceFile.Length}B");
        Console.WriteLine($"        to: {CheckRelatedPath(encrypted_file.FullName)}");

        using var src_stream = SourceFile.OpenRead();
        using var dst_stream = encrypted_file.Create();

        using (var crypt_stream = aes.GetEncryptionStream(dst_stream))
            src_stream.CopyToStream(crypt_stream, SourceFile.Length);

        Console.WriteLine("Encrypted.");
    }

    public static void Decrypt(this FileInfo EncryptedFile, Aes aes)
    {
        var source_file_full_name = Path.GetFileNameWithoutExtension(EncryptedFile.FullName);
        var dest_file = new FileInfo(source_file_full_name);

        Console.WriteLine($"Decrypting: {CheckRelatedPath(EncryptedFile.FullName)}");
        Console.WriteLine($"        to: {CheckRelatedPath(dest_file.FullName)}");

        try
        {
            using var src_stream = EncryptedFile.OpenRead();
            using var dst_stream = dest_file.Create();

            using (var crypt_stream = aes.GetDecryptionStream(src_stream))
                crypt_stream.CopyToStream(dst_stream, EncryptedFile.Length);

            Console.WriteLine("Decrypted.");
        }
        catch (CryptographicException)
        {
            dest_file.Delete();
            Console.WriteLine();
            Console.WriteLine("File name error");
        }
    }
}
