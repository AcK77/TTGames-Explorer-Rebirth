using System.Collections.Generic;
using System.Numerics;
using System.Reflection.PortableExecutable;
using TTGamesExplorerRebirthLib.Helper;
using static TTGamesExplorerRebirthLib.Formats.PinnerConvexMeshItem;

namespace TTGamesExplorerRebirthLib.Formats
{
    public class PinnerString
    {
        public List<string> Strings = [];

        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            if (version < 2)
            {
                throw new NotSupportedException($"{reader.BaseStream.Position:x8}");
            }

            for (int i = 0; i < count; i++)
            {
                Strings.Add(reader.ReadSized32NullTerminatedString());
            }
        }
    }

    public class PinnerMatrix
    {
        public List<List<float>> Matrices = [];

        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            for (int i = 0; i < count; i++)
            {
                List<float> matrix = [];

                // TODO: Doesn't seems to be a float in all cases.
                for (int j = 0; j < 16; j++)
                {
                    matrix.Add(reader.ReadSingleBigEndian());
                }

                Matrices.Add(matrix);
            }
        }
    }

    public class PinnerEntityItem
    {
        public float   Mass;
        public float   LinearDamping;
        public float   AngularDamping;
        public ushort  NameIndex;
        public ushort  TransformIndex;
        public byte    MotionType;
        public byte    AddToScene;
        public byte    PostAnimState;
        public byte    MassMode;
        public byte    HasController;
        public byte    Type;
        public byte    InertiaMode;
        public byte    TerrainLayer;
        public Vector3 Inertia;
        public byte[]  LockedVerts; // Size 8.
        public byte    NumLockedVerts;
        public byte    StopRotation;
        public byte    Scalable;
        public byte    IsPhantom;
        public byte    FloatHack;
        public byte    AiAvoidable;
        public ushort  CustomCom;
        public ushort  LocalCom;
        public uint    PlatformMask;
        public uint    CollisionLayers;
        public uint    CollisionLayersFilter;
        public ushort  ClothId;
        public byte    FluidType;
        public byte    UndulatingSurface;
        public uint    FlowStrength;
    }

    public class PinnerEntity
    {
        public List<PinnerEntityItem> Items = [];

        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            for (int i = 0; i < count; i++)
            {
                PinnerEntityItem item = new()
                {
                    Mass                  = reader.ReadSingleBigEndian(),
                    LinearDamping         = reader.ReadSingleBigEndian(),
                    AngularDamping        = reader.ReadSingleBigEndian(),
                    NameIndex             = reader.ReadUInt16BigEndian(),
                    TransformIndex        = reader.ReadUInt16BigEndian(),
                    MotionType            = reader.ReadByte(),
                    AddToScene            = reader.ReadByte(),
                    PostAnimState         = reader.ReadByte(),
                    MassMode              = reader.ReadByte(),
                    HasController         = reader.ReadByte(),
                    Type                  = reader.ReadByte(),
                    InertiaMode           = reader.ReadByte(),
                    TerrainLayer          = reader.ReadByte(),
                    Inertia               = new Vector3(reader.ReadSingleBigEndian(), reader.ReadSingleBigEndian(), reader.ReadSingleBigEndian()),
                    NumLockedVerts        = reader.ReadByte(),
                    LockedVerts           = reader.ReadBytes(8),
                    StopRotation          = reader.ReadByte(),
                    Scalable              = reader.ReadByte(),
                    CustomCom             = reader.ReadUInt16BigEndian(),
                    LocalCom              = reader.ReadUInt16BigEndian(),
                    PlatformMask          = reader.ReadUInt32BigEndian(),
                    CollisionLayers       = reader.ReadUInt32BigEndian(),
                    CollisionLayersFilter = reader.ReadUInt32BigEndian(),
                    ClothId               = reader.ReadUInt16BigEndian(),
                    FloatHack             = reader.ReadByte(),
                    FluidType             = reader.ReadByte(),
                    IsPhantom             = reader.ReadByte(),
                    UndulatingSurface     = reader.ReadByte(),
                    AiAvoidable           = reader.ReadByte(),
                };

                if (version > 13)
                {
                    item.FlowStrength = reader.ReadUInt32BigEndian();
                    byte unknown = reader.ReadByte(); // Always 0 ?
                }

                Items.Add(item);
            }
        }
    }

    public class PinnerMesh
    {
        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            if (count != 0)
            {
                throw new NotImplementedException($"{reader.BaseStream.Position:x8}");
            }

            for (int i = 0; i < count; i++)
            {
                // TODO: Size is unknown.
            }
        }
    }

    public class PinnerJoint
    {
        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            if (count != 0)
            {
                throw new NotImplementedException($"{reader.BaseStream.Position:x8}");
            }

            for (int i = 0; i < count; i++)
            {
                // TODO: Size is unknown.
            }
        }
    }

    public class PinnerGeomItem
    {
        public Vector3 Extents;
        public float   ShellRadius;
        public uint    MeshDepth;
        public ushort  BodyIndex;
        public ushort  TransformIndex;
        public ushort  MeshIndex;
        public ushort  TerrainExtra;
        public byte    TerrainType;
        public byte    GeomFlags;
        public byte    Material;
        public byte    Type;
        public byte    IsInstance;
        public byte    IsSecondaryShape;
        public uint    PlatformMask;
    }

    public class PinnerGeom
    {
        public List<PinnerGeomItem> Items;

        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            Items = [];

            for (int i = 0; i < count; i++)
            {
                PinnerGeomItem item = new PinnerGeomItem
                {
                    Extents     = new Vector3(reader.ReadSingleBigEndian(), reader.ReadSingleBigEndian(), reader.ReadSingleBigEndian()),
                    ShellRadius = reader.ReadSingleBigEndian()
                };

                if (version != 1)
                {
                    item.MeshDepth = reader.ReadUInt32BigEndian();
                }

                item.BodyIndex        = reader.ReadUInt16BigEndian();
                item.TransformIndex   = reader.ReadUInt16BigEndian();
                item.MeshIndex        = reader.ReadUInt16BigEndian();
                item.TerrainExtra     = reader.ReadUInt16BigEndian();
                item.TerrainType      = reader.ReadByte();
                item.GeomFlags        = reader.ReadByte();
                item.Material         = reader.ReadByte();
                item.Type             = reader.ReadByte();
                item.IsInstance       = reader.ReadByte();
                item.IsSecondaryShape = reader.ReadByte();

                if (version != 1 && version >= 3)
                {
                    item.PlatformMask = reader.ReadUInt32BigEndian();
                }

                Items.Add(item);
            }
        }
    }

    public class PinnerSpline
    {
        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            if (count != 0)
            {
                throw new NotImplementedException($"{reader.BaseStream.Position:x8}");
            }

            for (int i = 0; i < count; i++)
            {
                // TODO: Size is unknown.
            }
        }
    }

    public class PinnerController
    {
        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            if (count != 0)
            {
                throw new NotImplementedException($"{reader.BaseStream.Position:x8}");
            }

            for (int i = 0; i < count; i++)
            {
                // TODO: Size is unknown.
            }
        }
    }

    public class PinnerTileMap
    {
        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            if (count != 0)
            {
                throw new NotImplementedException($"{reader.BaseStream.Position:x8}");
            }

            for (int i = 0; i < count; i++)
            {
                // TODO: Size is unknown.
            }
        }
    }

    public class PinnerConvexMeshItem
    {
        public class PinnerConvexMeshVertItem
        {
            public float X;
            public float Y;
            public float Z;
            public ushort NumNeighbours;
            public ushort VertIndex;
        }

        public class PinnerConvexMeshNormalsItem
        {
            public ushort NumPoints;
            public ushort IndeciesIndex;
        }

        public float  Covariance;
        public ushort NumVertNeighbours;
        public ushort NumPolys;
        public ushort NumVerts;
        public PinnerConvexMeshVertItem[] Verts;
        public ushort NumNormals;
        public PinnerConvexMeshNormalsItem[] Normals;
        public ushort[] Indices;
        public ushort[] VertFaces;
        public ushort[] VertNeighbours;

        public ushort[] Unknown; // if (version == 1 && NumVerts > 1)
    }

    public class PinnerConvexMesh
    {
        List<List<PinnerConvexMeshVertItem>> Verts = [];
        List<List<PinnerConvexMeshVertItem>> VertsNeighbours = [];

        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            for (int i = 0; i < count; i++)
            {
                reader.ReadBytes(0x40); // HalfF array.

                byte[] counter = reader.ReadBytes(0xA);

                int firstArray = counter[1];
                int secondArray = counter[3];
                int thirdArray = counter[5];
                int fourthArray = counter[7];
                int fifthArray = counter[9];

                List<PinnerConvexMeshVertItem> verts = [];

                for (int j = 0; j < fourthArray; j++)
                {
                    verts.Add(new PinnerConvexMeshVertItem()
                    {
                        X = reader.ReadSingleBigEndian(),
                        Y = reader.ReadSingleBigEndian(),
                        Z = reader.ReadSingleBigEndian(),
                        NumNeighbours = reader.ReadUInt16BigEndian(),
                        VertIndex = reader.ReadUInt16BigEndian(),
                    });
                }

                Verts.Add(verts);

                List<PinnerConvexMeshVertItem> vertsNeighbours = [];

                for (int j = 0; j < thirdArray; j++)
                {
                    vertsNeighbours.Add(new PinnerConvexMeshVertItem()
                    {
                        X = reader.ReadSingleBigEndian(),
                        Y = reader.ReadSingleBigEndian(),
                        Z = reader.ReadSingleBigEndian(),
                        NumNeighbours = reader.ReadUInt16BigEndian(),
                        VertIndex = reader.ReadUInt16BigEndian(),
                    });
                }

                VertsNeighbours.Add(vertsNeighbours);

                // TODO.
                
                reader.ReadBytes(firstArray * 7);
                
                if (secondArray == 0)
                {
                    reader.ReadBytes(6);
                }
            }
        }
    }

    public class KdTerrainDescriptor
    {
        public void Deserialize(BinaryReader reader)
        {
            ushort count   = reader.ReadUInt16BigEndian();
            uint   version = reader.ReadUInt32BigEndian();

            if (count != 0)
            {
                throw new NotImplementedException($"{reader.BaseStream.Position:x8}");
            }

            for (int i = 0; i < count; i++)
            {
                // TODO: Size is unknown.
            }
        }
    }

    public class PinnerDynamicsObj
    {
        private const string MagicSerialised = "SERIALISED";

        public PinnerDynamicsObj(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public PinnerDynamicsObj(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            // Read header.

            uint fileSize = reader.ReadUInt32BigEndian();

            if (reader.ReadNullTerminatedString() != MagicSerialised)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint fileVersion = reader.ReadUInt32BigEndian(); // Always 20 ?

            // There are 12 sections here.

            new PinnerString().Deserialize(reader);
            new PinnerMatrix().Deserialize(reader);
            new PinnerEntity().Deserialize(reader);
            new PinnerMesh().Deserialize(reader);
            new PinnerJoint().Deserialize(reader);
            new PinnerGeom().Deserialize(reader);
            new PinnerSpline().Deserialize(reader);
            new PinnerController().Deserialize(reader);
            new PinnerTileMap().Deserialize(reader);
            new PinnerConvexMesh().Deserialize(reader);
            new KdTerrainDescriptor().Deserialize(reader);
            new PinnerMesh().Deserialize(reader);

            if (stream.Position != stream.Length)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }
        }
    }
}