using System.Runtime.InteropServices;
using TTGamesExplorerRebirthLib.Formats.NuCore;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
#pragma warning disable IDE0059
    public class EdStreamInfo
    {
        public EdTypeList  TypeList;
        public EdClassList ClassList;

        public EdStreamInfo(BinaryReader reader)
        {
            uint   size    = reader.ReadUInt32();
            string name    = reader.ReadSized32NullTerminatedString(true);
            uint   version = reader.ReadUInt32();

            if (name != "StreamInfo")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            if (version < 31)
            {
                byte unknown1 = reader.ReadByte(); // Always 1 ?
            }
            else if (version > 50)
            {
                ulong unknown1 = reader.ReadUInt64();

                new EdResourceTimeStampGUID().Deserialize(reader);
                new EdGraphType().Deserialize(reader);
                new EdBakeContributingResources().Deserialize(reader);

                uint unknownSize1 = reader.ReadUInt32();
                uint unknownSize2 = reader.ReadUInt32();
            }

            TypeList  = new EdTypeList(reader);
            ClassList = new EdClassList(reader, version, TypeList);
        }
    }

    public class EdResourceTimeStampGUID
    {
        public void Deserialize(BinaryReader reader)
        {
            uint   size = reader.ReadUInt32();
            string name = reader.ReadSized32NullTerminatedString(true);

            if (name != "ResourceTimeStampGUID")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            byte[] guid = reader.ReadBytes(0x10);
        }
    }

    public class EdGraphType
    {
        public void Deserialize(BinaryReader reader)
        {
            uint   size = reader.ReadUInt32();
            string name = reader.ReadSized32NullTerminatedString(true);

            if (name != "GraphType")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint unknown = reader.ReadUInt32();
        }
    }

    public class EdBakeContributingResources
    {
        public void Deserialize(BinaryReader reader)
        {
            uint   size    = reader.ReadUInt32();
            string name    = reader.ReadSized32NullTerminatedString(true);
            uint   version = reader.ReadUInt32();
            uint   count   = reader.ReadUInt32();

            if (name != "BakeContributingResources")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            for (uint i = 0; i < count; i++)
            {
                byte[] guidLow      = reader.ReadBytes(0x10);
                byte[] guidHigh     = reader.ReadBytes(0x10);
                string resourceName = reader.ReadSized32NullTerminatedString(true);
            }
        }
    }

    public class EdTypeList
    {
        public List<EdType> Types = [];

        public EdTypeList(BinaryReader reader)
        {
            uint   size  = reader.ReadUInt32();
            string name  = reader.ReadSized32NullTerminatedString(true);
            uint   count = reader.ReadUInt32();

            if (name != "TypeList")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            for (uint i = 0; i < count; i++)
            {
                Types.Add(new EdType(reader));
            }
        }
    }

    public class EdType
    {
        public string Name;
        public int    Size;

        public EdType(BinaryReader reader)
        {
            uint   size = reader.ReadUInt32();
            string name = reader.ReadSized32NullTerminatedString(true);

            if (name != "Type")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            Name = reader.ReadSized32NullTerminatedString(true);

            if (Name == "")
            {
                Name = "Null";
            }

            Size = reader.ReadInt32();
        }
    }

    public class EdClassList
    {
        public List<EdClass> Classes = [];

        public EdClassList(BinaryReader reader, uint streamInfoVersion, EdTypeList typeList)
        {
            uint   size = reader.ReadUInt32();
            string name = reader.ReadSized32NullTerminatedString(true);

            if (name != "ClassList")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint count = reader.ReadUInt32();

            for (uint i = 0; i < count; i++)
            {
                Classes.Add(new EdClass(reader, streamInfoVersion));
            }

            for (int i = 0; i < count; i++)
            {
                foreach (EdClass c in Classes)
                {
                    foreach (EdClassType type in  c.Objects)
                    {
                        if (type.UseClassType)
                        {
                            type.Type = Classes[(int)type.TypeIndex];
                        }
                        else
                        {
                            type.Type = typeList.Types[(int)type.TypeIndex];
                        }
                    }

                    if (c.Components != null)
                    {
                        foreach (EdComponent component in c.Components.Components)
                        {
                            component.Type = typeList.Types[(int)component.TypeIndex];
                        }
                    }

                    if (c.Params != null)
                    {
                        // TODO.
                    }
                }
            }
        }
    }

    public class EdClass
    {
        public string       Name;
        public float        Version;
        public EdParams     Params;
        public EdComponents Components;

        public List<EdClassType> Objects = [];

        public EdClass(BinaryReader reader, uint streamInfoVersion)
        {
            long position = reader.BaseStream.Position;

            uint   classSize = reader.ReadUInt32();
            string name      = reader.ReadSized32NullTerminatedString(true);

            if (name != "Class")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            Name = reader.ReadSized32NullTerminatedString(true);

            uint size = reader.ReadUInt32();

            name = reader.ReadSized32NullTerminatedString(true);

            if (name != "Version")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            Version = reader.ReadSingle();

            size = reader.ReadUInt32();
            name = reader.ReadSized32NullTerminatedString(true);

            if (name != "Types")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            if (streamInfoVersion > 50)
            {
                byte unknown = reader.ReadByte();
            }

            uint count = reader.ReadUInt32();

            for (uint i = 0; i < count; i++)
            {
                Objects.Add(new EdClassType(reader));
            }

            for (uint i = 0; i < 2; i++)
            {
                if (position + classSize > reader.BaseStream.Position)
                {
                    long oldPosition = reader.BaseStream.Position;

                    size = reader.ReadUInt32();
                    name = reader.ReadSized32NullTerminatedString(true);

                    reader.BaseStream.Seek(oldPosition, SeekOrigin.Begin);

                    if (name == "Params")
                    {
                        Params = new EdParams(reader);
                    }
                    else if (name == "Components")
                    {
                        Components = new EdComponents(reader);
                    }
                    else
                    {
                        throw new NotSupportedException($"{reader.BaseStream.Position:x8}");
                    }
                }
            }
        }
    }

    public class EdParams
    {
        public uint Value;

        public EdParams(BinaryReader reader)
        {
            uint   size = reader.ReadUInt32();
            string name = reader.ReadSized32NullTerminatedString(true);

            if (name != "Params")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            Value = reader.ReadUInt32();
        }
    }

    public class EdComponents
    {
        public List<EdComponent> Components = [];

        public EdComponents(BinaryReader reader)
        {
            uint size = reader.ReadUInt32();

            if (size != 0)
            {
                string name = reader.ReadSized32NullTerminatedString(true);

                if (name != "Components")
                {
                    throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
                }

                uint count = reader.ReadUInt32();

                for (uint i = 0; i < count; i++)
                {
                    Components.Add(new EdComponent(reader));
                }
            }
        }
    }

    public class EdComponent
    {
        public string Name;
        public uint   TypeIndex;
        public object Type;

        public EdComponent(BinaryReader reader)
        {
            uint   size = reader.ReadUInt32();
            string name = reader.ReadSized32NullTerminatedString(true);

            if (name != "Component")
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            Name      = reader.ReadSized32NullTerminatedString(true);
            TypeIndex = reader.ReadUInt32();
        }
    }

    public class EdClassType
    {
        public struct EdClassTypeFlags
        {
            public byte Unknown0;
            public byte Unknown1;
            public byte Unknown2;
            public byte Unknown3;
            public byte Unknown4;
            public byte Unknown5;
            public byte Unknown6;
            public byte Unknown7;
            public byte Unknown8;
            public byte Unknown9;
            public byte Unknown10;
            public byte Unknown11;
            public byte Unknown12;
            public byte Unknown13;
            public byte Unknown14;
            public byte Unknown15;
            public int  Unknown16;
        }

        public uint             TypeIndex;
        public object           Type;
        public string           Name;
        public EdClassTypeFlags Flags;
        public bool             UseClassType;
        public bool             IsArray;
        public bool             IsSkipped;

        public EdClassType(BinaryReader reader)
        {
            TypeIndex = reader.ReadUInt32();
            Name      = reader.ReadSized32NullTerminatedString(true);
            
            byte[] flags = reader.ReadBytes(0x14);

            Flags = MemoryMarshal.Cast<byte, EdClassTypeFlags>(flags)[0];

            UseClassType = (Flags.Unknown11 & 128) != 0;
            IsArray      = Flags.Unknown16 == 0;
            IsSkipped    = Flags.Unknown14 != 0;
        }
    }

    public class ApiRegistry
    {
        private const string ObjectsList = "OLST";
        private const string MObject     = "MOBJ";

        private readonly BinaryReader _reader;
        private readonly EdStreamInfo _streamInfo;



        public ApiRegistry(BinaryReader reader, EdStreamInfo streamInfo)
        {
            _reader     = reader;
            _streamInfo = streamInfo;

            DeserialiseClassObjectsIn();
        }

        private void DeserialiseClassObjectsIn()
        {
            uint objectListCount = _reader.ReadUInt32();

            for (int i = 0; i < objectListCount; i++)
            {
                uint size = _reader.ReadUInt32();

                if (_reader.ReadSized32NullTerminatedString(true) != ObjectsList)
                {
                    throw new InvalidDataException($"{_reader.BaseStream.Position:x8}");
                }

                DeserialiseObjectIn();
            }
        }

        // Should be in ApiClass
        private void DeserialiseObjectIn()
        {
            ushort objectCount      = _reader.ReadUInt16();
            uint   edClassTypeIndex = _reader.ReadUInt32();

            for (int j = 0; j < objectCount; j++)
            {
                uint size = _reader.ReadUInt32();

                if (_reader.ReadSized32NullTerminatedString(true) != MObject)
                {
                    throw new InvalidDataException($"{_reader.BaseStream.Position:x8}");
                }

                EdClass c = _streamInfo.ClassList.Classes[(int)edClassTypeIndex];
                
                foreach (EdClassType edClassType in c.Objects)
                {
                    if (edClassType.IsSkipped)
                    {
                        continue;
                    }

                    if (edClassType.UseClassType)
                    {
                        foreach (EdClassType edType in ((EdClass)edClassType.Type).Objects)
                        {
                            switch (edType.Name)
                            {
                                case "Char":
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
        }
    }

    public class EdFile
    {
        public EdStreamInfo StreamInfo;
        public ApiRegistry  ApiRegistry;

        public EdFile(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public EdFile(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            if (new NuFileHeader().IsNuFile(reader))
            {
                new NuFileHeader().Deserialize(reader);
                new NuResourceHeader().Deserialize(reader);
            }

            StreamInfo  = new(reader);
            ApiRegistry = new(reader, StreamInfo);
        }
    }
#pragma warning restore IDE0059
}