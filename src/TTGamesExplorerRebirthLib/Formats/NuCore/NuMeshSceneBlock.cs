using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuMeshSceneBlock
    {
        public const string Magic = "HSEM";

        public NuRenderMesh NuRenderMesh;

        public NuMeshSceneBlock Deserialize(BinaryReader reader)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint nuMeshSceneBlockVersion = reader.ReadUInt32BigEndian();
            if (nuMeshSceneBlockVersion >= 0xAD)
            {
                uint size = reader.ReadUInt32BigEndian(); // Size of what ?

                NuRenderMesh = new NuRenderMesh().Deserialize(reader, nuMeshSceneBlockVersion);
            }
            else if (nuMeshSceneBlockVersion <= 0x2F)
            {
                return null;
            }
            else
            {
                if (reader.ReadUInt32AsString() != NuVector.Magic)
                {
                    throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
                }

                // TODO.
                uint id   = reader.ReadUInt32BigEndian();
                uint size = reader.ReadUInt32BigEndian();

                NuRenderMesh = new NuRenderMesh().Deserialize(reader, nuMeshSceneBlockVersion);
            }

            return this;
        }
    }
#pragma warning restore IDE0059
}