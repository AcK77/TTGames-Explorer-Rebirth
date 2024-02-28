using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuResourceHeader
    {
        public static string Magic = "HSER";

        public uint             Version            { get; private set; }
        public uint             Type               { get; private set; }
        public string           ProjectName        { get; private set; }
        public string           ProducedByUserName { get; private set; }
        public string           SourceFileName     { get; private set; }
        public NuFileTreeNode[] Nodes              { get; private set; }

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

            Version = reader.ReadUInt32BigEndian();
            Type    = reader.ReadUInt32BigEndian();
            
            if (Type == 1)
            {
                Nodes = new NuFileTree().Deserialize(reader);
            }

            new NuVector().Deserialize(reader, this);

            ProjectName = reader.ReadSized16NullTerminatedString();

            uint resourceType       = reader.ReadUInt32();
            uint accurevTransaction = reader.ReadUInt32();

            ProducedByUserName = reader.ReadSized16NullTerminatedString();

            sbyte unknown1 = reader.ReadSByte();

            SourceFileName = reader.ReadSized16NullTerminatedString();

            sbyte unknown2 = reader.ReadSByte();

            return this;
        }
    }
#pragma warning restore IDE0059
}