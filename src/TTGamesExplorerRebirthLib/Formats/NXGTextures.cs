using TTGamesExplorerRebirthLib.Formats.DDS;
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
        private const string Magic     = ".CC4";
        private const string MagicResH = "HSER";
        private const string MagicVtor = "ROTV";
        private const string MagicTxSt = "TSXT";

        public string ProjectName;
        public string ProducedByUserName;
        public string SourceFileName;
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

            uint nuResourceHeaderSize = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicResH)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicResH)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint nuResourceHeaderVersion = reader.ReadUInt32BigEndian();
            uint unknown1 = reader.ReadUInt32();

            if (reader.ReadUInt32AsString() != MagicVtor)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint vtorVersion = reader.ReadUInt32BigEndian();
            uint unknown2    = reader.ReadUInt32();

            ushort projectNameSize = reader.ReadUInt16BigEndian();
            ProjectName = stream.ReadNullTerminatedString();

            uint unknown3 = reader.ReadUInt32();
            uint unknown4 = reader.ReadUInt32();

            ushort producedByUserNameSize = reader.ReadUInt16BigEndian();
            ProducedByUserName = stream.ReadNullTerminatedString();

            byte unknown5 = reader.ReadByte();

            ushort sourceFileNameSize = reader.ReadUInt16BigEndian();
            SourceFileName = stream.ReadNullTerminatedString();

            byte unknown6 = reader.ReadByte();

            // Read NuTextureSetHeader.

            uint nuTextureSetHeaderSize = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicTxSt)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint nuTextureSetHeaderUnknown1 = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != MagicTxSt)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint   nuTextureSetHeaderVersion  = reader.ReadUInt32BigEndian();
            ushort nuTextureSetHeaderUnknown2 = reader.ReadUInt16BigEndian();

            ushort dateStampSize = reader.ReadUInt16BigEndian();
            DateStamp = stream.ReadNullTerminatedString();

            if (reader.ReadUInt32AsString() != MagicVtor)
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

                if (nuTextureSetHeaderVersion == 1)
                {
                    uint pathSize            = reader.ReadUInt32BigEndian();
                         path                = stream.ReadNullTerminatedString();
                    uint nuTexGenHdrUnknown1 = reader.ReadUInt32BigEndian();
                }
                
                if (nuTextureSetHeaderVersion == 12)
                {
                    uint nuAlignedBuffer = reader.ReadUInt32();
                    byte pathSize        = reader.ReadByte();
                         path            = stream.ReadNullTerminatedString();
                    byte nuResourceId    = reader.ReadByte();
                }

                // FIXME: Some nxg_textures failed here.
                if (nuTextureSetHeaderVersion == 14)
                {
                    if (!isNuChecksumZeroed)
                    {
                        uint nuAlignedBuffer     = reader.ReadUInt32();
                        byte pathSize            = reader.ReadByte();
                             path                = stream.ReadNullTerminatedString();
                        byte nuResourceId        = reader.ReadByte();
                        uint nuTexGenHdrUnknown3 = reader.ReadUInt32();
                        uint nuTexGenHdrUnknown4 = reader.ReadUInt32();
                    }
                    else
                    {
                        ushort pathSize            = reader.ReadUInt16BigEndian();
                               path                = stream.ReadNullTerminatedString();
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