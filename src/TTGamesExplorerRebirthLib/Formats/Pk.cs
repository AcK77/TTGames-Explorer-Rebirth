using System.Reflection;
using System.Text;
using TTGamesExplorerRebirthLib.Compression;
using TTGamesExplorerRebirthLib.Hashes;

namespace TTGamesExplorerRebirthLib.Formats
{
    public class PkFile
    {
        public uint   DecompressedSize;
        public uint   CompressedSize;
        public uint   Offset;
        public uint   Hash;
        public string Path;
        public byte[] Data;
    }

    /// <summary>
    ///     Give pki/pkd path and deserialize it to be able to extract files.
    /// </summary>
    /// <remarks>
    ///     Based on my own research (Ac_K).
    ///     Thanks to KimochiYikes for the help.
    /// </remarks>
    public class Pk
    {
        public List<PkFile> Files = [];

        public Pk(string indexPath, string dataPath)
        {
            // Read the index file.

            using FileStream   indexStream = new(indexPath, FileMode.Open, FileAccess.Read);
            using BinaryReader indexReader = new(indexStream);

            ulong indexMagic = indexReader.ReadUInt64();
            if (indexMagic != 3)
            {
                throw new NotSupportedException();
            }

            Dictionary<uint, string> filePaths = [];

            using StreamReader pathsStreamReader = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("TTGamesExplorerRebirthLib.Resources.LHPC_pk_paths.txt"));
            string line;
            while ((line = pathsStreamReader.ReadLine()) != null)
            {
                filePaths.TryAdd(Fnv.Fnv1_32_PKWin(line), line);
            }

            uint indexFileCount = indexReader.ReadUInt32();
            for (int i = 0; i < indexFileCount; i++)
            {
                PkFile pkWinFile = new()
                {
                    DecompressedSize = indexReader.ReadUInt32(),
                    CompressedSize   = indexReader.ReadUInt32(),
                    Offset           = indexReader.ReadUInt32(),
                    Hash             = indexReader.ReadUInt32(),
                };

                if (filePaths.TryGetValue(pkWinFile.Hash, out string path))
                {
                    pkWinFile.Path = path;
                }

                Files.Add(pkWinFile);
            }

            uint eof = indexReader.ReadUInt32();
            if (eof != 0)
            {
                throw new EndOfStreamException();
            }

            // Read the data file.

            using FileStream   dataStream = new(dataPath, FileMode.Open, FileAccess.Read);
            using BinaryReader dataReader = new(dataStream);

            ulong dataMagic = dataReader.ReadUInt64();
            if (dataMagic != 3)
            {
                throw new NotSupportedException();
            }

            uint dataFileCount = dataReader.ReadUInt32();
            if (dataFileCount != indexFileCount)
            {
                throw new InvalidDataException();
            }

            foreach (PkFile file in Files)
            {
                if (file.CompressedSize != 0)
                {
                    dataStream.Seek(file.Offset + 2, SeekOrigin.Begin); // Skip the Inflate header.

                    file.Data = Inflate.Decompress(dataReader.ReadBytes((int)file.CompressedSize - 2));
                }
                else
                {
                    dataStream.Seek(file.Offset, SeekOrigin.Begin);

                    file.Data = dataReader.ReadBytes((int)file.DecompressedSize); ;
                }

                if (file.Path == null)
                {
                    file.Path = $"{file.Offset:X8}{Helper.Helper.GetExtensionByMagic(Encoding.ASCII.GetString(file.Data[..4]))}";
                }
            }
        }
    }
}