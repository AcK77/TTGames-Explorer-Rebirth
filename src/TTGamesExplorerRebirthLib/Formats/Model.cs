using TTGamesExplorerRebirthLib.Formats.NuCore;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    public class Model
    {
        public NuResourceHeader ReasourceHeader;
        public NuScene          Scene;

        public Model(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public Model(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            // Read header.

            new NuFileHeader().Deserialize(reader);

            ReasourceHeader = new NuResourceHeader().Deserialize(reader);

            // Read NuScene.

            uint nuSceneSize    = reader.ReadUInt32BigEndian();
            uint nuSceneVersion = reader.ReadUInt32BigEndian(); // Always 1 ?

            Scene = new NuScene().Deserialize(reader, ReasourceHeader);

            // TODO.
        }
    }
}