namespace FileEncryptor;

internal static class Constants
{
    public const string EncodedExt = ".aes";

    private static byte[] GetSlat()
    {
        var asm_name = Path.GetFileNameWithoutExtension(Environment.ProcessPath) ?? "";
        var asm_name_bytes = Encoding.UTF8.GetBytes(asm_name);
        var slat = asm_name_bytes.Concat((byte[])[0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c]).Take(16).ToArray();
        return slat;
    }

    public static readonly byte[] Salt = GetSlat();
    //[
    //    0x26, 0xdc, 0xff, 0x00,
    //    0xad, 0xed, 0x7a, 0xee,
    //    0xc5, 0xfe, 0x07, 0xaf,
    //    0x4d, 0x08, 0x22, 0x3c,
    //];

    public static string CurrentPath { get; } = AppContext.BaseDirectory;
}