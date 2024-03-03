using TTGamesExplorerRebirthLib.Formats.DDS;
using TTGamesExplorerRebirthLib.Formats.NuCore;

namespace TTGamesExplorerRebirthLib.Formats
{
    /// <summary>
    ///     Give nxg_texture file data and deserialize it.
    /// </summary>
    /// <remarks>
    ///     NuTexGenHdr versioning by Jay Franco:
    ///     https://github.com/Smakdev/NuTCracker
    ///     
    ///     Based on my own research (Ac_K).
    /// </remarks>
    public class NXGTextures
    {
        public NuTextureSet TextureSet;

        public Dictionary<string, byte[]> Files;

        public NXGTextures(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public NXGTextures(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            new NuFileHeader().Deserialize(reader);
            new NuResourceHeader().Deserialize(reader);
            
            TextureSet = new NuTextureSet().Deserialize(reader);

            List<string> filesPath = new NuTexGenHdr().Deserialize(reader, TextureSet.Version).FilesPath;

            Files = [];

            for (int i = 0; i < filesPath.Count; i++)
            {
                uint ddsSize = DDSImage.CalculateDdsSize(stream, reader);

                Files.Add(filesPath[i], reader.ReadBytes((int)ddsSize));

                if (stream.Position == stream.Length)
                {
                    Files = Files.Take(i + 1).Skip(Files.Count - (i + 1)).ToDictionary();

                    break;
                }
            }
        }
    }
}