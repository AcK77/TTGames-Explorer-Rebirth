using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public class NuScene
    {
        private const string Magic          = "02UN";
        private const string MagicNameTable = "LBTN";

        public NuConversionInfo   ConversionInfo { get; private set; }
        public List<string>       NameTable      { get; private set; }
        public NuTexHdrSceneBlock TexHdrScene    { get; private set; }
        public NuMeshSceneBlock   NuMeshScene    { get; private set; }

        public NuScene Deserialize(BinaryReader reader, NuResourceHeader nuResourceHeader)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint sceneVersion = reader.ReadUInt32BigEndian();

            ConversionInfo = new NuConversionInfo().Deserialize(reader);

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
                TexHdrScene = new NuTexHdrSceneBlock().Deserialize(reader, texHdrSceneBlockVersion);
            }

            // TODO: Find differencies between DeserializeVector<NuSpline,4>() and DeserializeVectorFromVarArray<NuSpline,4>()
            if (nameTableVersion > 19)
            {
                NuSpline[] splines = NuVector.Deserialize<NuSpline[]>(reader, nuResourceHeader);
            }
            else
            {
                NuSpline[] splines = NuVector.Deserialize<NuSpline[]>(reader, nuResourceHeader);
            }

            // TODO.
            /*
            if (nameTableVersion <= 68) // && somethingElse )
            {
                // NOTE: It reads a string array.
                throw new NotSupportedException($"{reader.BaseStream.Position:x8}");
            }
            */

            NuVFXLocator vfxLocatorSet = NuVector.Deserialize<NuVFXLocator>(reader, nuResourceHeader);

            if (nameTableVersion <= 77)
            {
                // TODO: Find differencies between DeserializeVector<short,4>() and DeserializeVectorFromVarArray<short,4>()
                if (nameTableVersion > 19)
                {
                    ushort[] deprecatedInstanceIxs = NuVector.Deserialize<ushort[]>(reader, nuResourceHeader);
                }
                else
                {
                    ushort[] instanceIxs = NuVector.Deserialize<ushort[]>(reader, nuResourceHeader);
                }
            }

            uint nuMeshSceneBlockSize = reader.ReadUInt32BigEndian();
            if (nuMeshSceneBlockSize != 0)
            {
                NuMeshScene = new NuMeshSceneBlock().Deserialize(reader);
            }

            uint nuMtlSceneBlockSize = reader.ReadUInt32BigEndian();
            if (nuMtlSceneBlockSize != 0)
            {
                NuMtlSceneBlock nuMeshSceneBlock = new NuMtlSceneBlock().Deserialize(reader);
            }

            return this;
        }
    }
}
