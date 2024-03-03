using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public class NuVertexBuffer
    {
        public List<List<NuVertex>> Vertices;

        public NuVertexBuffer Deserialize(BinaryReader reader, uint nuMeshSceneBlockVersion)
        {
            uint size = reader.ReadUInt32BigEndian(); // Or version ?

            if (size != 0)
            {
                uint flags = reader.ReadUInt32BigEndian();

                // TODO: Flags bits operations are from the serializer, so we should do the invert.
                /*
                if (nuMeshSceneBlockVersion < 0xAA)
                {
                    flags |= 2;
                }

                if (nuMeshSceneBlockVersion < 0xA9)
                {
                    flags &= 0xFFFFFEFF;
                }

                if (nuMeshSceneBlockVersion < 0xAC)
                {
                    flags &= 0xFFFFF1FF;
                    flags |= 0x200;
                }

                int  somethingType = -1;
                uint somethingFlag = (flags >> 8) & 0xFFFFFF01;

                if ((flags & 0x200) != 0)
                {
                    somethingType = 0;
                }
                else if ((flags & 0x400) != 0)
                {
                    somethingType = 1;
                }
                else if ((flags & 0x800) != 0)
                {
                    somethingType = 2;
                }
                */

                uint numVertex = reader.ReadUInt32BigEndian();

                Vertices = [];

                NuVertexDesc nuVertexDescription = new NuVertexDesc().Deserialize(reader, nuMeshSceneBlockVersion);

                for (int i = 0; i < numVertex; i++)
                {
                    List<NuVertex> nuVertices = [];

                    foreach (var desc in nuVertexDescription.Attributes)
                    {
                        switch (desc.Type)
                        {
                            case NuVertexDescAttributeType.Null:
                                {
                                    // Do nothing?
                                    break;
                                }

                            case NuVertexDescAttributeType.Float1:
                                {
                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = reader.ReadSingle(),
                                    });
                                    break;
                                }

                            case NuVertexDescAttributeType.Float2:
                                {
                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = new Vector2(reader.ReadSingle(),
                                                                 reader.ReadSingle()),
                                    });
                                    break;
                                }

                            case NuVertexDescAttributeType.Float3:
                                {
                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = new Vector3(reader.ReadSingle(),
                                                                 reader.ReadSingle(),
                                                                 reader.ReadSingle()),
                                    });
                                    break;
                                }

                            case NuVertexDescAttributeType.Float4:
                                {
                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = new Vector4(reader.ReadSingle(),
                                                                 reader.ReadSingle(),
                                                                 reader.ReadSingle(),
                                                                 reader.ReadSingle()),
                                    });
                                    break;
                                }

                            case NuVertexDescAttributeType.Half2:
                                {
                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = new Vector2((float)BitConverter.Int16BitsToHalf(reader.ReadInt16()),
                                                                 (float)BitConverter.Int16BitsToHalf(reader.ReadInt16())),
                                    });
                                    break;
                                }

                            case NuVertexDescAttributeType.Half4:
                                {
                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = new Vector4((float)BitConverter.Int16BitsToHalf(reader.ReadInt16()),
                                                                 (float)BitConverter.Int16BitsToHalf(reader.ReadInt16()),
                                                                 (float)BitConverter.Int16BitsToHalf(reader.ReadInt16()),
                                                                 (float)BitConverter.Int16BitsToHalf(reader.ReadInt16())),
                                    });
                                    break;
                                }

                            case NuVertexDescAttributeType.UByte4:
                                {
                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = new Vector4(reader.ReadByte(),
                                                                 reader.ReadByte(),
                                                                 reader.ReadByte(),
                                                                 reader.ReadByte()),
                                    });
                                    break;
                                }

                            case NuVertexDescAttributeType.UByteN4:
                                {
                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = new Vector4(reader.ReadSByte(),
                                                                 reader.ReadSByte(),
                                                                 reader.ReadSByte(),
                                                                 reader.ReadSByte()),
                                    });
                                    break;
                                }

                            case NuVertexDescAttributeType.Color:
                                {
                                    byte b = reader.ReadByte();
                                    byte g = reader.ReadByte();
                                    byte r = reader.ReadByte();
                                    byte a = reader.ReadByte();

                                    nuVertices.Add(new NuVertex()
                                    {
                                        Definition = desc.Definition,
                                        Type       = desc.Type,
                                        Value      = new Rgba32(r, g, b, a),
                                    });
                                    break;
                                }

                            default: throw new NotSupportedException($"{reader.BaseStream.Position:x8}");
                        }
                    }

                    Vertices.Add(nuVertices);
                }
            }

            return this;
        }
    }
}