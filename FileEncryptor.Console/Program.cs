using FileEncryptor;


if (args is [])
{
    Console.WriteLine("No files to processing");
    return;
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

return;

static Aes CreateAes(string pass)
{
    var aes = Aes.Create();

    var keyAndIv = Rfc2898DeriveBytes.Pbkdf2(
        pass,
        Constants.Salt,
        13,
        HashAlgorithmName.SHA512,
        48);

    aes.Key = keyAndIv[..32];
    aes.IV = keyAndIv[32..];
    return aes;
}