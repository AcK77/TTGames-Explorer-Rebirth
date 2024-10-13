using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    public class ShaderBlobFile
    {
        public uint           Key;
        public uint           Sent;
        public ShaderBlobType Type;
        public byte[]         Data;
    }

    public enum ShaderBlobType
    {
        Vertex,
        Fragment,
    }

    /// <summary>
    ///     Give shader blob file data and deserialize it.
    /// </summary>
    /// <remarks>
    ///     Based on my own research (Ac_K).
    /// </remarks>
    public class ShaderBlob
    {
        private const string MagicShaderBlobVersion = "SBLBVERS";
        private const string MagicDebug             = "DBUG";
        private const string MagicData              = "DATA";
        private const string MagicSent              = "SENT";
        private const string MagicShaderKey         = "SKEY";
        private const string MagicShaderVPO         = "SVPO";
        private const string MagicShaderFPO         = "SFPO";

        public uint Version { get; private set; }
        public uint Debug   { get; private set; }
        public uint Data    { get; private set; }

        public List<ShaderBlobFile> Shaders = [];

        public ShaderBlob(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public ShaderBlob(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            // Read header.

            if (reader.ReadUInt64AsString() != MagicShaderBlobVersion)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            Version = reader.ReadUInt32();

            if (reader.ReadUInt32AsString() != MagicDebug)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            Debug = reader.ReadUInt32();

            if (reader.ReadUInt32AsString() != MagicData)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            Data = reader.ReadUInt32();

            // Read shader files.

            uint sent = 0;
            uint key  = 0;

            while (stream.Position < stream.Length)
            {
                string ident = reader.ReadUInt32AsString();

                if (ident == MagicSent)
                {
                    sent = reader.ReadUInt32();
                }
                else if (ident == MagicShaderKey)
                {
                    key = reader.ReadUInt32();
                }
                else if (ident == MagicShaderVPO || ident == MagicShaderFPO)
                {
                    ShaderBlobFile shaderBlobFile = new();

                    int shaderSize = reader.ReadInt32();

                    shaderBlobFile.Sent = sent;
                    shaderBlobFile.Key  = key;
                    shaderBlobFile.Type = ident == MagicShaderVPO ? ShaderBlobType.Vertex : ShaderBlobType.Fragment;
                    shaderBlobFile.Data = reader.ReadBytes(shaderSize);

                    Shaders.Add(shaderBlobFile);

                    sent = 0;
                    key  = 0;
                }
                else
                {
                    throw new InvalidDataException($"{stream.Position:x8}");
                }
            }
        }
    }
}
