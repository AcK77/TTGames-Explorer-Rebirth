using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public class NuTexGenHdr
    {
        public List<string> FilesPath { get; private set; }

        public NuTexGenHdr Deserialize(BinaryReader reader, uint nuTexHdrVersion)
        {
            if (nuTexHdrVersion == 14)
            {
                if (reader.ReadUInt32BigEndian() != 0) // VTOR is null.
                {
                    throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
                }
            }
            else
            {
                if (reader.ReadUInt32AsString() != NuVector.Magic)
                {
                    throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
                }
            }

            uint nuTextureCount = reader.ReadUInt32BigEndian();

            FilesPath = [];

            for (int i = 0; i < nuTextureCount; i++)
            {
                byte[] nuChecksum         = reader.ReadBytes(0x10);
                bool   isNuChecksumZeroed = !nuChecksum.Any(b => b != 0);

                string path = "";

                if (nuTexHdrVersion == 1 || nuTexHdrVersion == 0)
                {
                    path = reader.ReadSized32NullTerminatedString();

                    uint nuTextureType = reader.ReadUInt32BigEndian();
                }

                if (nuTexHdrVersion == 12)
                {
                    uint level = reader.ReadUInt32(); // Accurev internal level (unused)

                    path = reader.ReadSized8NullTerminatedString();

                    byte nuTextureTypeAsU8 = reader.ReadByte();
                }

                // FIXME: Some textures failed here.
                if (nuTexHdrVersion == 14)
                {
                    if (!isNuChecksumZeroed)
                    {
                        reader.ReadSized16NullTerminatedString();

                        path = reader.ReadSized16NullTerminatedString();

                        byte nuTextureTypeAsU8 = reader.ReadByte();

                        NuTextureType textureType = (NuTextureType)reader.ReadUInt32BigEndian() - 1;

                        ushort unknown3 = reader.ReadUInt16BigEndian();
                        ushort unknown4 = reader.ReadUInt16BigEndian();
                    }
                    else
                    {
                        path = reader.ReadSized16NullTerminatedString();

                        ushort unknown1 = reader.ReadUInt16BigEndian();
                        ushort unknown2 = reader.ReadUInt16BigEndian();
                        uint   unknown3 = reader.ReadUInt32BigEndian();
                        ushort unknown4 = reader.ReadUInt16BigEndian();
                        ushort unknown5 = reader.ReadUInt16BigEndian();
                    }
                }

                if (!isNuChecksumZeroed)
                {
                    // TODO: Add more informations.
                    FilesPath.Add(path);
                }
            }

            return this;
        }
    }
}
