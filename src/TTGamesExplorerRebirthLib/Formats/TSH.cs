using TTGamesExplorerRebirthLib.Formats.DDS;
using TTGamesExplorerRebirthLib.Formats.NuCore;

namespace TTGamesExplorerRebirthLib.Formats
{
    /// <summary>
    ///     Give tsh file (NuTextureSheet) data and deserialize it.
    /// </summary>
    /// <remarks>
    ///     Based on my own research (Ac_K).
    ///     
    ///     Some field names found by Jay Franco.
    /// </remarks>
    public class TSH
    {
        public NuTextureSheet TextureSheet { get; private set; }
        public DDSImage       Image        { get; private set; }

        public TSH(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public TSH(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            new NuFileHeader().Deserialize(reader);
            new NuResourceHeader().Deserialize(reader);

            TextureSheet = new NuTextureSheet().Deserialize(reader);
            Image        = new DDSImage(reader.ReadBytes((int)(stream.Length - (int)stream.Position)));
        }
    }
}