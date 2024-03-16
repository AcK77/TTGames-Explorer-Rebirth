using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuResourceHeader
    {
        public const string Magic = "HSER";

        public uint     Version            { get; private set; }
        public uint     Type               { get; private set; }
        public string   ProjectName        { get; private set; }
        public string   ProducedByUserName { get; private set; }
        public string   SourceFileName     { get; private set; }
        public string[] Files              { get; private set; }

        public NuResourceHeader Deserialize(BinaryReader reader)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            Version = reader.ReadUInt32BigEndian(); // Name is maybe wrong?

            if (Version >= 17) // Maybe it could be lower.
            {
                string fileName   = reader.ReadSized16NullTerminatedString();
                byte[] nuChecksum = reader.ReadBytes(0x10);
            }

            Type = reader.ReadUInt32BigEndian(); // Name is maybe wrong?

            if (Type == 1)
            {
                Files = new NuFileTree().Deserialize(reader).Files;
            }

            NuVector.Deserialize<NuResourceReference>(reader, this);

            ProjectName = reader.ReadSized16NullTerminatedString();

            uint resourceType       = reader.ReadUInt32();
            uint accurevTransaction = reader.ReadUInt32();

            ProducedByUserName = reader.ReadSized16NullTerminatedString();

            sbyte unknown1 = reader.ReadSByte();

            SourceFileName = reader.ReadSized16NullTerminatedString();

            if (Version >= 17) // Maybe it could be lower.
            {
                byte[] nuChecksum1 = reader.ReadBytes(0x10);
                uint   unknown2    = reader.ReadUInt32BigEndian();
                uint   fnv1aHash1  = reader.ReadUInt32BigEndian();
                byte[] nuChecksum2 = reader.ReadBytes(0x10);
                byte[] nuChecksum3 = reader.ReadBytes(0x10);
                uint   unknown3    = reader.ReadUInt32BigEndian();
                uint   fnv1aHash2  = reader.ReadUInt32BigEndian();
            }

            sbyte unknown5 = reader.ReadSByte();

            return this;
        }
    }
#pragma warning restore IDE0059
}