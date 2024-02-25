using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    public class PCShadersFile
    {
        public PCShadersType Type;
        public byte[]        Data;
    }

    public enum PCShadersType
    {
        DXBC,
        CTAB,
    }

    /// <summary>
    ///     Give pc_shaders file data and deserialize it.
    /// </summary>
    /// <remarks>
    ///     Based on my own research (Ac_K).
    /// </remarks>
    public class PCShaders
    {
        private const string Magic     = ".CC4";
        private const string MagicResH = "HSER";
        private const string MagicVtor = "ROTV";
        private const string MagicBcsh = "HSCB";

        public string ProjectName;

        public List<PCShadersFile> Shaders = [];

        public PCShaders(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public PCShaders(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            // Read header.

            uint nuResourceHeaderSize = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicResH)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicResH)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint unknown1 = reader.ReadUInt32BigEndian(); // Always 8 ?
            uint unknown2 = reader.ReadUInt32BigEndian(); // Always 0 ?

            if (reader.ReadUInt32AsString() != MagicVtor)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint   unknown3  = reader.ReadUInt32BigEndian(); // Always 0 ?
            uint   unknown4  = reader.ReadUInt32BigEndian(); // Always 14 ?
            ushort unknown5  = reader.ReadUInt16BigEndian(); // Always 1 ?
            ushort unknown6  = reader.ReadUInt16BigEndian(); // Always 0 ?
            uint   unknown7  = reader.ReadUInt32BigEndian(); // Always 0 ?
            uint   unknown8  = reader.ReadUInt32BigEndian(); // Always 0 ?
            byte   unknown9  = reader.ReadByte(); // Always 1 ?
            byte   unknown10 = reader.ReadByte(); // Always 0 ?
            byte   unknown11 = reader.ReadByte(); // Always 0xFF ?

            ushort projectNameSize = reader.ReadUInt16BigEndian();
            ProjectName = stream.ReadNullTerminatedString();

            byte unknown12 = reader.ReadByte(); // Always 2 ?

            // Read subheader.

            uint dataSize  = reader.ReadUInt32BigEndian(); // dataSize + nuResourceHeaderSize + 8 (2 int for sizes) = Total filesize.

            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicResH)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicResH)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint unknown13 = reader.ReadUInt32BigEndian(); // Always 2 ?
            uint unknown14 = reader.ReadUInt32BigEndian(); // Always 0x808 ?

            if (reader.ReadUInt32AsString() != MagicBcsh)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint unknown15 = reader.ReadUInt32BigEndian(); // Always 12 ?
            uint unknown16 = reader.ReadUInt32BigEndian(); // It's the total of shader files / 2 (Why?)

            // Read shader files.

            int i = 0;
            while (stream.Position < stream.Length - 4)
            {
                stream.Seek((i % 2 == 0) ? 0x1A : 0xA, SeekOrigin.Current);
                
                ushort        shaderSize = reader.ReadUInt16BigEndian();
                PCShadersType shaderType = PCShadersType.DXBC;

                if (reader.ReadUInt16().ToConvertedString() != "DX")
                {
                    stream.Seek(-2, SeekOrigin.Current);
                    reader.ReadByte();

                    if (reader.ReadByte() != 3)
                    {
                        throw new InvalidDataException($"{stream.Position:x8}");
                    }

                    shaderType = PCShadersType.CTAB;
                }

                stream.Seek(-2, SeekOrigin.Current);

                Shaders.Add(new()
                {
                    Type = shaderType,
                    Data = reader.ReadBytes(shaderSize),
                });

                i++;
            }

            uint unknown17 = reader.ReadUInt32BigEndian(); // Each file finish with a value here.
        }
    }
}