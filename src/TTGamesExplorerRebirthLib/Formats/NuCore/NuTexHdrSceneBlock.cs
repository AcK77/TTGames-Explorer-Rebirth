using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuTexHdrSceneBlock
    {
        public const string Magic = "HGXT";

        public List<string> FilesPath {  get; private set; }

        public NuTexHdrSceneBlock Deserialize(BinaryReader reader, uint texHdrSceneBlockVersion)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint version = reader.ReadUInt32BigEndian();

            if (version <= 10)
            {
                if (reader.ReadUInt32AsString() != NuVector.Magic)
                {
                    throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
                }

                uint size = reader.ReadUInt32BigEndian();
            }

            FilesPath = new NuTexGenHdr().Deserialize(reader, texHdrSceneBlockVersion).FilesPath;

            return this;
        }
    }
#pragma warning restore IDE0059
}
