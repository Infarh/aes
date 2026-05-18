using FileEncryptor;

if (args is [])
{
    Console.WriteLine("No files to processing");
    return 0;
}

var has_errors = false;
foreach (var file_name in args)
{
    var file = new FileInfo(file_name);
    if (!file.Exists)
    {
        Console.Error.WriteLine($"File not found: {file_name}");
        has_errors = true;
        continue;
    }

    try
    {
        var ext = file.Extension;
        if (string.Equals(ext, Constants.EncodedExt, StringComparison.OrdinalIgnoreCase))
        {
            var password = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name));
            file.Decrypt(password);
        }
        else
        {
            var password = Path.GetFileNameWithoutExtension(file.Name);
            file.Encrypt(password);
        }
    }
    catch (InvalidDataException error)
    {
        has_errors = true;
        Console.Error.WriteLine($"Format error for '{file.Name}': {error.Message}");
    }
    catch (CryptographicException ex)
    {
        has_errors = true;
        Console.Error.WriteLine($"Cryptographic error for '{file.Name}': {ex.Message}");
    }
    catch (Exception ex)
    {
        has_errors = true;
        Console.Error.WriteLine($"Processing error for '{file.Name}': {ex.Message}");
    }
}

return has_errors ? 1 : 0;