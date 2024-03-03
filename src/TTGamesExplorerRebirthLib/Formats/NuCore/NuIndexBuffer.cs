using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuIndexBuffer
    {
        public ushort[] Indices { get; private set; }

        public NuIndexBuffer Deserialize(BinaryReader reader)
        {
            uint size = reader.ReadUInt32BigEndian();
            if (size != 0)
            {
                uint flags          = reader.ReadUInt32BigEndian();
                uint count          = reader.ReadUInt32BigEndian();
                uint indexSizeAsU32 = reader.ReadUInt32BigEndian();

                Indices = new ushort[count];

                // TODO: Flags bits operations are from the serializer, so we should do the invert.
                /*
                if (nuMeshSceneBlockVersion <= 0xA9)
                {
                    flags -= 2;

                    if (nuMeshSceneBlockVersion != 0xA9)
                    {
                        flags &= 0xFFFFFEFF;
                    }
                }
                */

                for (int i = 0; i < count; i++)
                {
                    // TODO: There is an issue with indices in some cases.
                    byte b1 = reader.ReadByte();
                    byte b2 = reader.ReadByte();

                    if (b1 != 0)
                    {
                        Indices[i] = b1;
                    }
                    else
                    {
                        Indices[i] = b2;
                    }
                }
            }

            return this;
        }
    }
#pragma warning restore IDE0059
}