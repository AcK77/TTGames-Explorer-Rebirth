namespace TTGamesExplorerRebirthLib.Formats.DAT
{
    public class DATFile
    {
        public string            Path;
        public ulong             Offset;
        public uint              Size;
        public uint              CompressedSize;
        public CompressionFormat Compression;

        public DATFile(string path, ulong offset, uint size, uint compressedSize, CompressionFormat compression)
        {
            Path        = path;
            Offset      = offset;
            Size        = size;
            Compression = compression;

            if (compression != CompressionFormat.None)
            {
                CompressedSize = compressedSize;
            }
        }

        public override string ToString()
        {
            string value = $"Path: {Path}\n\tOffset: 0x{Offset:X8}\n\tSize: 0x{Size:X8}\n";

            if (Compression != CompressionFormat.None)
            {
                value += $"\tCompressedSize: 0x{CompressedSize:X8} ({Compression})\n";
            }

            return value;
        }
    }
}