using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuRenderMeshVbArrayItem
    {
        public NuVertexBuffer NuVertexBuffer;

        public NuRenderMeshVbArrayItem Deserialize(BinaryReader reader, uint nuMeshSceneBlockVersion)
        {
            NuVertexBuffer = new NuVertexBuffer().Deserialize(reader, nuMeshSceneBlockVersion);

            uint byteOffset = reader.ReadUInt32BigEndian();

            return this;
        }
    }
#pragma warning restore IDE0059
}