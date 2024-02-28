using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public class NuScene
    {
#pragma warning disable IDE0059
        private const string Magic          = "02UN";
        private const string MagicNameTable = "LBTN";

        public NuConversionInfo   ConversionInfo   { get; private set; }
        public List<string>       NameTable        { get; private set; }
        public NuTexHdrSceneBlock TexHdrSceneBlock { get; private set; }

        public NuScene Deserialize(BinaryReader reader, NuResourceHeader nuResourceHeader)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint sceneVersion = reader.ReadUInt32BigEndian();

            ConversionInfo = new NuConversionInfo().Deserialize(reader, nuResourceHeader);

            if (reader.ReadUInt32AsString() != MagicNameTable)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            NameTable = [];

            uint nameTableVersion     = reader.ReadUInt32BigEndian();
            uint nameTableSize        = reader.ReadUInt32BigEndian();
            long nameTableFinalOffset = reader.BaseStream.Position + nameTableSize;

            while (reader.BaseStream.Position != nameTableFinalOffset)
            {
                NameTable.Add(reader.ReadNullTerminatedString());
            }

            uint texHdrSceneBlockVersion = reader.ReadUInt32BigEndian();

            if (texHdrSceneBlockVersion != 0)
            {
                TexHdrSceneBlock = new NuTexHdrSceneBlock().Deserialize(reader, texHdrSceneBlockVersion);
            }

            return this;
        }
#pragma warning restore IDE0059
    }
}
