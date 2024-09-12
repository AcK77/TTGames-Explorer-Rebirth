namespace TTGamesExplorerRebirthLib.Formats.FIB
{
    public class FibFile
    {
        public string            Path;
        public uint              Hash;
        public uint              Offset;
        public uint              Flags;
        public uint              Size; // NOTE: Can be decompressed size if compressed.
        public CompressionFormat Compression;

        public FibFile(uint hash, uint offset, uint flags, uint size, CompressionFormat compression)
        {
            Hash        = hash;
            Path        = ""; // TODO: Add files path table and search the right one into it based on the hash.
            Offset      = offset;
            Flags       = flags;
            Size        = size;
            Compression = compression;
        }

        public override string ToString()
        {
            string value = $"\tHash: {Hash:X8}\n\tPath: {Path}\n\tOffset: 0x{Offset:X8}\n\tFlags: 0x{Flags:X8}\n\tSize: 0x{Size:X8}\n";

            if (Compression != CompressionFormat.None)
            {
                value += $"\tCompression: {Compression}\n";
            }

            return value;
        }
    }
}
