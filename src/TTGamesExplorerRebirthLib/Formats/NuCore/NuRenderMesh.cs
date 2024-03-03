using System.Numerics;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuRenderMesh
    {
        public NuRenderMeshVbArrayItem[] NuRenderMeshVbArray { get; private set; }
        public NuIndexBuffer[]           NuIndexBufferArray  { get; private set; }

        public NuRenderMesh Deserialize(BinaryReader reader, uint nuMeshSceneBlockVersion)
        {
            uint nuIndexBufferSize  = reader.ReadUInt32BigEndian();
            uint nuRenderMeshVbSize = reader.ReadUInt32BigEndian();

            NuRenderMeshVbArray = new NuRenderMeshVbArrayItem[nuRenderMeshVbSize];
            NuIndexBufferArray  = new NuIndexBuffer[nuIndexBufferSize];

            for (uint i = 0; i < nuRenderMeshVbSize; i++)
            {
                NuRenderMeshVbArray[i] = new NuRenderMeshVbArrayItem().Deserialize(reader, nuMeshSceneBlockVersion);
            }

            uint unknown1 = reader.ReadUInt32BigEndian();

            for (uint i = 0; i < nuIndexBufferSize; i++)
            {
                NuIndexBufferArray[i] = new NuIndexBuffer().Deserialize(reader);
            }

            // TODO: It's wrong in some cases.

            uint indexBufferOffset = reader.ReadUInt32BigEndian();
            uint indexBufferCount  = reader.ReadUInt32BigEndian();
            uint indexBufferBase   = reader.ReadUInt32BigEndian();

            uint   unknown2 = reader.ReadUInt32BigEndian();
            ushort primType = reader.ReadUInt16BigEndian();

            uint vbUsedCount     = reader.ReadUInt32BigEndian();
            uint vbInstBitsAsU32 = reader.ReadUInt32BigEndian();

            uint   skinMtxMap = reader.ReadUInt32BigEndian();
            byte[] unknown3   = reader.ReadBytes(0x10);

            Vector3 centreExtents0 = new(reader.ReadSingleBigEndian(), reader.ReadSingleBigEndian(), reader.ReadSingleBigEndian());
            Vector3 centreExtents1 = new(reader.ReadSingleBigEndian(), reader.ReadSingleBigEndian(), reader.ReadSingleBigEndian());

            uint defunctOptFlags = reader.ReadUInt32BigEndian();
            // uint densityDiscDiameter = reader.ReadUInt32BigEndian();

            return this;
        }
    }
#pragma warning restore IDE0059
}