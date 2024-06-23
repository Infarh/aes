using System.Buffers;

namespace FileEncryptor;

internal static class StreamEx
{
    private const int __BufferSize = 1024 * 1024;

    public static void CopyToStream(this Stream Src, Stream Dest, long TotalLength)
    {
        var buffer_array = ArrayPool<byte>.Shared.Rent(__BufferSize);

        try
        {
            var total_bytes = 0d;
            var buffer = buffer_array.AsSpan(0, __BufferSize);
            while (Src.Read(buffer) is (> 0 and var read_bytes))
            {
                Dest.Write(buffer[..read_bytes]);

                total_bytes += read_bytes;
                Console.CursorLeft = 0;
                Console.Write("  {0:p2} {1} / {2} B", total_bytes / TotalLength, total_bytes, TotalLength);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer_array);
        }
        Console.WriteLine();
    }
}
