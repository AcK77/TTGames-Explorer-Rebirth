using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public class NuMtlSceneBlock
    {
        public const string Magic = "LTMU";

        public NuMtlSceneBlock Deserialize(BinaryReader reader)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                // throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            // TODO.

            return this;
        }
    }
}