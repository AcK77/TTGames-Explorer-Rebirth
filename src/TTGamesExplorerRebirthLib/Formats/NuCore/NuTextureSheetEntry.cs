using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public class NuTextureSheetEntry
    {
        public float MinU;
        public float MinV;
        public float MaxU;
        public float MaxV;

        public uint MinX;
        public uint MinY;
        public uint Width;
        public uint Height;

        public string Magic;

        public int TrimTop;
        public int TrimBottom;
        public int TrimLeft;
        public int TrimRight;

        // NOTE: The name hash is a NuStrHashUpperCaseFNV1 from a ToC in "filename_content.txt" file.
        public uint NameHash;

        public NuTextureSheetEntry Deserialize(BinaryReader reader)
        {
            // TODO: Handle other version.

            MinU = reader.ReadSingleBigEndian();
            MinV = reader.ReadSingleBigEndian();
            MaxU = reader.ReadSingleBigEndian();
            MaxV = reader.ReadSingleBigEndian();

            MinX = reader.ReadUInt32BigEndian();
            MinY = reader.ReadUInt32BigEndian();

            Width = reader.ReadUInt32BigEndian();
            Height = reader.ReadUInt32BigEndian();

            Magic = reader.ReadSizedString(4);

            TrimTop    = reader.ReadInt32BigEndian();
            TrimBottom = reader.ReadInt32BigEndian();
            TrimLeft   = reader.ReadInt32BigEndian();
            TrimRight  = reader.ReadInt32BigEndian();

            NameHash = reader.ReadUInt32();

            return this;
        }
    }
}