using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuTextureSet
    {
        private const string MagicTxSt = "TSXT";

        public uint   Size;
        public uint   Version;
        public string DateStamp;

        public NuTextureSet Deserialize(BinaryReader reader)
        {
            Size = new NuFileHeader().Deserialize(reader);

            if (reader.ReadUInt32AsString() != MagicTxSt)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint TXSTHDRCount = reader.ReadUInt32BigEndian(); // always 1

            if (reader.ReadUInt32AsString() != MagicTxSt)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            Version = reader.ReadUInt32BigEndian();
            if (Version > 0)
            {
                DateStamp = reader.ReadSized32NullTerminatedString();
            }

            return this;
        }
    }
#pragma warning restore IDE0059
}