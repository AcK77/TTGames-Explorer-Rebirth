using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuTexGenHdr
    {
        public List<string> FilesPath { get; private set; }

        public NuTexGenHdr Deserialize(BinaryReader reader, uint nuTexHdrVersion)
        {
            if (reader.ReadUInt32AsString() != NuVector.Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint nuTextureCount = reader.ReadUInt32BigEndian();

            FilesPath = [];

            for (int i = 0; i < nuTextureCount; i++)
            {
                // Read NuTexGenHdr.

                byte[] nuChecksum = reader.ReadBytes(0x10);
                bool isNuChecksumZeroed = !nuChecksum.Any(b => b != 0);

                string path = "";

                if (nuTexHdrVersion == 1 || nuTexHdrVersion == 0)
                {
                    path = reader.ReadSized32NullTerminatedString();
                    uint nuTexGenHdrUnknown1 = reader.ReadUInt32BigEndian();
                }

                if (nuTexHdrVersion == 12)
                {
                    uint nuAlignedBuffer = reader.ReadUInt32();
                    path = reader.ReadSized8NullTerminatedString();
                    byte nuResourceId = reader.ReadByte();
                }

                // FIXME: Some textures failed here.
                if (nuTexHdrVersion == 14)
                {
                    if (!isNuChecksumZeroed)
                    {
                        uint nuAlignedBuffer = reader.ReadUInt32();
                        path = reader.ReadSized8NullTerminatedString();
                        byte nuResourceId = reader.ReadByte();
                        uint nuTexGenHdrUnknown3 = reader.ReadUInt32();
                        uint nuTexGenHdrUnknown4 = reader.ReadUInt32();
                    }
                    else
                    {
                        path = reader.ReadSized16NullTerminatedString();
                        uint nuTexGenHdrUnknown1 = reader.ReadUInt32();
                        uint nuTexGenHdrUnknown2 = reader.ReadUInt32();
                        uint nuTexGenHdrUnknown3 = reader.ReadUInt32();
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
#pragma warning restore IDE0059
}
