using TTGamesExplorerRebirthLib.Formats.NuCore;
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
        private const string MagicBcsh = "HSCB";

        public NuResourceHeader ResourceHeader;

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

            new NuFileHeader().Deserialize(reader);

            ResourceHeader = new NuResourceHeader().Deserialize(reader);

            // Read subheader.

            uint dataSize = new NuFileHeader().Deserialize(reader); // dataSize + nuResourceHeaderSize + 8 (2 int for sizes) = Total filesize.

            if (reader.ReadUInt32AsString() != NuResourceHeader.Magic)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != NuResourceHeader.Magic)
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