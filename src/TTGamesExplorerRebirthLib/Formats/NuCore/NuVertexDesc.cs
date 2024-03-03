using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuVertexDesc
    {
        public const string Magic = "DXTV";

        public NuVertexDescAttribute[] Attributes { get; private set; }

        public NuVertexDesc Deserialize(BinaryReader reader, uint nuMeshSceneBlockVersion)
        {
            if (nuMeshSceneBlockVersion >= 0xA1)
            {
                if (reader.ReadUInt32AsString() != Magic)
                {
                    throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
                }

                uint nuVertexDescVersion = reader.ReadUInt32BigEndian();

                if (nuVertexDescVersion <= 0x2D)
                {
                    Attributes = new NuVertexDescAttribute[16];

                    for (int i = 0; i < 16; i++)
                    {
                        Attributes[i] = new NuVertexDescAttribute()
                        {
                            Definition = (NuVertexDescAttributeDefinition)i,
                            Type       = (NuVertexDescAttributeType)reader.ReadByte(),
                            Offset     = reader.ReadByte(),
                        };
                    }
                }
                else
                {
                    uint numAttributes = reader.ReadUInt32BigEndian();

                    Attributes = new NuVertexDescAttribute[numAttributes];

                    for (int i = 0; i < numAttributes; i++)
                    {
                        Attributes[i] = new NuVertexDescAttribute()
                        {
                            Definition = (NuVertexDescAttributeDefinition)reader.ReadByte(),
                            Type       = (NuVertexDescAttributeType)reader.ReadByte(),
                            Offset     = reader.ReadByte(),
                        };
                    }
                }

                byte[] unknown1 = reader.ReadBytes(6);
            }

            return this;
        }
    }
#pragma warning restore IDE0059
}