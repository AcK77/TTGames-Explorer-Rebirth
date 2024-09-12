using TTGamesExplorerRebirthLib.Compression;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.FIB
{
    /// <summary>
    ///     Give FIB Archive data, get the list of the contained files and extract it if needed.
    /// </summary>
    /// <remarks>
    ///     Based from RE by Ac_K.
    ///     Based on fib_ex by yukinogatari:
    ///     https://github.com/yukinogatari/Reverse-Engineering
    ///     Thanks to Toadster172 for helping pointed out a lot of useful informations.
    /// </remarks>
    public class FibArchive
    {
        public const string MagicFuse = "FUSE1.00";

        public List<FibFile> Files = [];

        public string ArchiveFilePath;

        public FibArchive(string archiveFilePath)
        {
            ArchiveFilePath = archiveFilePath;

            using FileStream   stream = new(ArchiveFilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(stream);

            if (reader.ReadUInt64AsString() != MagicFuse)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint filesCount     = reader.ReadUInt32();
            uint foldersCount   = reader.ReadUInt32();
            uint filesTocOffset = reader.ReadUInt32(); // Include the header.

            long filesDataOffset = stream.Position;

            stream.Seek(filesTocOffset, SeekOrigin.Begin);

            for (int i = 0; i < filesCount; i++)
            {
                uint hash   = reader.ReadUInt32();
                uint offset = reader.ReadUInt32();
                uint flags  = reader.ReadUInt32();

                // TODO: This needs improvement to support all FIB archives versions.
                Files.Add(new FibFile(hash, offset, flags, flags >> 5, (CompressionFormat)(flags & 3)));
            }
        }

        /// <summary>
        ///     Extract FibFile from the Fib archive.
        /// </summary>
        /// <param name="file">
        ///     FibFile object representing the file to be extracted.
        /// </param>
        /// <param name="plainData">
        ///     Bool to decrypt/decompress the file while it's extracted or not.
        /// </param>
        /// <returns>
        ///     Byte array representing the file data.
        /// </returns>
        public byte[] ExtractFile(FibFile file, bool plainData = false)
        {
            using FileStream   stream = new(ArchiveFilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(stream);

            stream.Seek(file.Offset, SeekOrigin.Begin);

            uint size = file.Size;

            if (file.Compression == CompressionFormat.Refpack)
            {
                // NOTE: Refpack compressed data start with the size of the compressed data.
                //       "size" still contains the decompressed size, but we don't use it for decomp so we can replace
                //       it to avoid repetitive code.
                size = reader.ReadUInt32();
            }

            byte[] fileData = new byte[size];

            stream.Read(fileData, 0, (int)size);

            if (plainData)
            {
                switch (file.Compression)
                {
                    case CompressionFormat.Inflate:
                        {
                            fileData = Inflate.Decompress(fileData);
                            break;
                        }

                    case CompressionFormat.Refpack:
                        {
                            fileData = UnRefpack.Decompress(fileData);
                            break;
                        }
                }
            }

            return fileData;
        }
    }
}
