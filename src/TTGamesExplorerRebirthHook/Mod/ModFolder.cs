using System.Collections.Generic;
using System.IO;
using System.Linq;
using TTGamesExplorerRebirthHook.Games;
using TTGamesExplorerRebirthHook.Utils;

namespace TTGamesExplorerRebirthHook.Mod
{
    public static class ModFolder
    {
        public static List<ModFile> Files { get; private set; }

        public static void LoadFiles()
        {
            Files = new List<ModFile>();

            foreach (string path in Directory.GetFiles(TTGamesContants.ModsFolder, "*.*", SearchOption.AllDirectories))
            {
                string newPath = path.Replace("/", "\\").ToLowerInvariant();

                Files.Add(new ModFile()
                {
                    Path = newPath,
                    Size = (int)new FileInfo(newPath).Length
                });
            }
        }

        public static bool IsFileModded(string originalPath) => Files.Where(modFile => modFile.Path.Remove(0, TTGamesContants.ModsFolder.Length + 1) == originalPath.Replace("/", "\\").ToLowerInvariant()).Count() == 1;

        public static ModFile GetModdedFile(string originalPath) => Files.Where(modFile => modFile.Path.Remove(0, TTGamesContants.ModsFolder.Length + 1) == originalPath.Replace("/", "\\").ToLowerInvariant()).FirstOrDefault();
    }
}
