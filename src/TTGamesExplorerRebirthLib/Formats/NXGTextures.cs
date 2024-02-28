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

        public NuResourceHeader Header;

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

            reader.ReadNuFileHeader();

            Header = reader.ReadNuResourceHeader();

            // Read NuTextureSetHeader.

            uint nuTextureSetHeaderSize = reader.ReadNuFileHeader();

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

            if (reader.ReadUInt32AsString() != NuFile.MagicVector)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint nuTextureCount = reader.ReadUInt32BigEndian();

            for (int i = 0; i < nuTextureCount; i++)
            {
                // Read NuTexGenHdr.

                byte[] nuChecksum         = reader.ReadBytes(0x10);
                bool   isNuChecksumZeroed = !nuChecksum.Any(b => b != 0);

                string path = "";

                if (nuTextureSetHeaderVersion == 1 || nuTextureSetHeaderVersion == 0)
                {
                    uint pathSize            = reader.ReadUInt32BigEndian();
                         path                = reader.ReadNullTerminatedString();
                    uint nuTexGenHdrUnknown1 = reader.ReadUInt32BigEndian();
                }
                
                if (nuTextureSetHeaderVersion == 12)
                {
                    uint nuAlignedBuffer = reader.ReadUInt32();
                    byte pathSize        = reader.ReadByte();
                         path            = reader.ReadNullTerminatedString();
                    byte nuResourceId    = reader.ReadByte();
                }

                // FIXME: Some nxg_textures failed here.
                if (nuTextureSetHeaderVersion == 14)
                {
                    if (!isNuChecksumZeroed)
                    {
                        uint nuAlignedBuffer     = reader.ReadUInt32();
                        byte pathSize            = reader.ReadByte();
                             path                = reader.ReadNullTerminatedString();
                        byte nuResourceId        = reader.ReadByte();
                        uint nuTexGenHdrUnknown3 = reader.ReadUInt32();
                        uint nuTexGenHdrUnknown4 = reader.ReadUInt32();
                    }
                    else
                    {
                        ushort pathSize            = reader.ReadUInt16BigEndian();
                               path                = reader.ReadNullTerminatedString();
                        uint   nuTexGenHdrUnknown1 = reader.ReadUInt32();
                        uint   nuTexGenHdrUnknown2 = reader.ReadUInt32();
                        uint   nuTexGenHdrUnknown3 = reader.ReadUInt32();
                    }
                }

                if (!isNuChecksumZeroed)
                {
                    Files.Add(new NXGFile()
                    {
                        Path = path,
                        // TODO: Add more informations.
                    });
                }
            }

            for (int i = 0; i < Files.Count; i++)
            {
                uint ddsSize = DDSImage.CalculateDdsSize(stream, reader);

                Files[i].Data = reader.ReadBytes((int)ddsSize);

                if (stream.Position == stream.Length)
                {
                    Files = Files.Take(i + 1).Skip(Files.Count - (i + 1)).ToList();

                    break;
                }
            }
        }
    }
}
