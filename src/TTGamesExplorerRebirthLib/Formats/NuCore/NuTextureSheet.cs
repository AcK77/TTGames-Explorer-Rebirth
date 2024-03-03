using System.Text;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuTextureSheet
    {
        private const string MagicTxSh = "HSXT";

        public List<NuTextureSheetEntry> Entries = [];

        public NuTextureSheet Deserialize(BinaryReader reader)
        {
            uint size = new NuFileHeader().Deserialize(reader);

            if (reader.ReadUInt32AsString() != MagicTxSh)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicTxSh)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint version = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != NuVector.Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint count = reader.ReadUInt32BigEndian();

            for (int i = 0; i < count; i++)
            {
                Entries.Add(new NuTextureSheetEntry().Deserialize(reader));
            }
            
            return this;
        }
    }
#pragma warning restore IDE0059
}