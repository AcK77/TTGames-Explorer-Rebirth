using TTGamesExplorerRebirthLib.Formats.DDS;
using TTGamesExplorerRebirthLib.Formats.NuCore;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    public class NXGFile
    {
        public string Path;
        public byte[] Data;
    }

    /// <summary>
    ///     Give nxg_texture file data and deserialize it.
    /// </summary>
    /// <remarks>
    ///     NuTexGenHdr versioning by Jay Franco:
    ///     https://github.com/Smakdev/NuTCracker
    ///     
    ///     Based on my own research (Ac_K).
    /// </remarks>
    public class NXGTextures
    {
        private const string MagicTxSt = "TSXT";

        public NuResourceHeader ResourceHeader;

        public string DateStamp;

        public List<NXGFile> Files = [];

        public NXGTextures(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public NXGTextures(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            // Read header.

            new NuFileHeader().Deserialize(reader);

            ResourceHeader = new NuResourceHeader().Deserialize(reader);

            // Read NuTextureSetHeader.

            uint nuTextureSetHeaderSize = new NuFileHeader().Deserialize(reader);

            if (reader.ReadUInt32AsString() != MagicTxSt)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint nuTextureSetHeaderUnknown1 = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != MagicTxSt)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint nuTextureSetHeaderVersion = reader.ReadUInt32BigEndian();

            if (nuTextureSetHeaderVersion > 0)
            {
                DateStamp = reader.ReadSized32NullTerminatedString();
            }

            List<string> filesPath = new NuTexGenHdr().Deserialize(reader, nuTextureSetHeaderVersion).FilesPath;

            Files = [];

            for (int i = 0; i < filesPath.Count; i++)
            {
                uint ddsSize = DDSImage.CalculateDdsSize(stream, reader);

                Files.Add(new NXGFile()
                {
                    Path = filesPath[i],
                    Data = reader.ReadBytes((int)ddsSize),
                });

                if (stream.Position == stream.Length)
                {
                    Files = Files.Take(i + 1).Skip(Files.Count - (i + 1)).ToList();

                    break;
                }
            }
        }
    }
}
