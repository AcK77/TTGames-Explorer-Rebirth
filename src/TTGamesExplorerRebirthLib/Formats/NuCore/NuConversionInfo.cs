using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public class NuConversionInfo
    {
#pragma warning disable IDE0059
        private const string Magic = "OFNI";

        public string UserName   { get; private set; }
        public string TimeDate   { get; private set; }
        public string LegoPartId { get; private set; }

        public NuConversionInfo Deserialize(BinaryReader reader)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint version = reader.ReadUInt32BigEndian();

            UserName = (version < 2) ? reader.ReadSized32NullTerminatedString() : reader.ReadSized16NullTerminatedString();
            TimeDate = (version < 2) ? reader.ReadSized32NullTerminatedString() : reader.ReadSized16NullTerminatedString();

            if (version > 1)
            {
                LegoPartId = reader.ReadSized16NullTerminatedString();
            }
            else
            {
                byte unknown = reader.ReadByte();
            }

            return this;
        }
#pragma warning restore IDE0059
    }
}
