using FileEncryptor;


if (args is [])
{
    Console.WriteLine("No files to processing");
    return;
}

foreach (var file in args.Select(file_name => new FileInfo(file_name)).Where(f => f.Exists))
{
    var ext = file.Extension;

    if (string.Equals(ext, Constants.EncodedExt, StringComparison.OrdinalIgnoreCase))
    {
        var password = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name));
        using var aes = CreateAES(password);
        file.Decrypt(aes);
    }
    else
    {
        var password = Path.GetFileNameWithoutExtension(file.Name);
        using var aes = CreateAES(password);
        file.Encrypt(aes);
    }
}

return;

static Aes CreateAES(string Pass)
{
    using var pdb = new Rfc2898DeriveBytes(Pass, Constants.Salt, 13, HashAlgorithmName.SHA512);
    var aes = Aes.Create();
    aes.Key = pdb.GetBytes(32);
    aes.IV = pdb.GetBytes(16);
    return aes;
}