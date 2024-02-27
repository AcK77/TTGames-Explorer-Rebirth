using DarkUI.Forms;
using FastColoredTextBoxNS;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using TTGamesExplorerRebirthLib.Formats.DAT;
using TTGamesExplorerRebirthUI.Forms;
using static TTGamesExplorerRebirthUI.Native;

namespace TTGamesExplorerRebirthUI
{
    public static class Helper
    {
        public static void EnableDarkModeTitle(IntPtr handle)
        {
            if (DwmSetWindowAttribute(handle, 19, [1], 4) != HResult.S_OK)
            {
                DwmSetWindowAttribute(handle, 20, [1], 4);
            }
        }

        public static string FormatSize(ulong bytesSize)
        {
            string[] sizeSuffixes = ["Bytes", "KB", "MB", "GB", "TB", "PB"];
            int      counter      = 0;
            decimal  number       = bytesSize;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return string.Format("{0:n1}{1}", number, sizeSuffixes[counter]);
        }

        public static Bitmap GetIconByFileName(string path)
        {
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".adp" => new Bitmap(Properties.Resources.page_white_music),
                ".ats" => new Bitmap(Properties.Resources.page_white_text),
                ".cbx" => new Bitmap(Properties.Resources.page_white_music),
                ".cfg" => new Bitmap(Properties.Resources.page_white_text),
                ".cmo" => new Bitmap(Properties.Resources.picture),
                ".csv" => new Bitmap(Properties.Resources.page_white_database),
                ".dat" => new Bitmap(Properties.Resources.box),
                ".dds" => new Bitmap(Properties.Resources.picture),
                ".dll" => new Bitmap(Properties.Resources.page_gear),
                ".exe" => new Bitmap(Properties.Resources.application),
                ".ft2" => new Bitmap(Properties.Resources.font),
                ".git" => new Bitmap(Properties.Resources.page_white_text),
                ".ini" => new Bitmap(Properties.Resources.page_white_text),
                ".lua" => new Bitmap(Properties.Resources.page_white_text),
                ".mp3" => new Bitmap(Properties.Resources.page_white_music),
                ".nxg_textures" => new Bitmap(Properties.Resources.pictures),
                ".ogg" => new Bitmap(Properties.Resources.page_white_music),
                ".pc_shaders" => new Bitmap(Properties.Resources.border_all),
                ".scp" => new Bitmap(Properties.Resources.page_white_text),
                ".sf" => new Bitmap(Properties.Resources.page_white_text),
                ".sub" => new Bitmap(Properties.Resources.page_white_text),
                ".subopt" => new Bitmap(Properties.Resources.page_white_text),
                ".tex" => new Bitmap(Properties.Resources.picture),
                ".tsh" => new Bitmap(Properties.Resources.picture),
                ".txt" => new Bitmap(Properties.Resources.page_white_text),
                ".pac" => new Bitmap(Properties.Resources.brick),
                ".pak" => new Bitmap(Properties.Resources.brick),
                ".wav" => new Bitmap(Properties.Resources.page_white_music),
                ".xml" => new Bitmap(Properties.Resources.page_code),

                _ => new Bitmap(Properties.Resources.page_white),
            };
        }

        public static void OpenFileInternal(string gameFolderPath, string path, byte[] fileBuffer = null, object archiveFile = null)
        {
            DarkForm form = null;
            switch (Path.GetExtension(path).ToLowerInvariant())
            {
                case ".dat":
                    {
                        form = new DATForm(gameFolderPath, path);
                        break;
                    }

                case ".pac":
                case ".pak":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        form = new PAKForm(path, fileBuffer);
                        break;
                    }

                case ".ats":
                case ".cfg":
                case ".csv":
                case ".git":
                case ".ini":
                case ".lua":
                case ".scp":
                case ".sf":
                case ".sub":
                case ".subopt":
                case ".txt":
                case ".xml":
                    {
                        /*
                        fileBuffer ??= File.ReadAllBytes(path);
                        
                        new TextForm(Path.GetFileName(path), fileBuffer, archiveFile).ShowDialog();
                        break;
                        */
                        if (fileBuffer == null)
                        {
                            break;
                        }
                        string temp = Path.GetTempPath() + "TTGEtemp.txt";

                        FileStream file = File.Create(temp);
                        file.Write(fileBuffer, 0, fileBuffer.Length);
                        file.Close();

                        Process.Start("notepad.exe", temp);
                        Task.Delay(10000).ContinueWith(t => File.Delete(temp));
                        break;
                    }

                case ".cmo":
                case ".dds":
                case ".tex":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        form = new ImageForm(path, fileBuffer, ImageFormType.DDS);
                        break;
                    }

                case ".nxg_textures":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);
                        
                        form = new ImageForm(path, fileBuffer, ImageFormType.NXGTextures);
                        break;
                    }

                case ".ft2":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        form = new FontForm(path, fileBuffer);
                        break;
                    }

                case ".tsh":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        form = new TSHForm(path, fileBuffer);
                        break;
                    }

                case ".adp":
                case ".cbx":
                case ".mp3":
                case ".ogg":
                case ".wav":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        form = new SoundForm(path, fileBuffer);
                        break;
                    }

                case ".pc_shaders":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        form = new PCShadersForm(path, fileBuffer);
                        break;

                    }
            }
            if (form != null)
            {
                form.ShowDialog();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
            }
        }
    }
}
