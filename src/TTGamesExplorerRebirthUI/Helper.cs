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

        static readonly private Bitmap bitmap_page_white_music = new Bitmap(Properties.Resources.page_white_music);
        static readonly private Bitmap bitmap_page_white_text = new Bitmap(Properties.Resources.page_white_text);
        static readonly private Bitmap bitmap_picture = new Bitmap(Properties.Resources.picture);
        static readonly private Bitmap bitmap_page_white_database = new Bitmap(Properties.Resources.page_white_database);
        static readonly private Bitmap bitmap_box = new Bitmap(Properties.Resources.box);
        static readonly private Bitmap bitmap_page_gear = new Bitmap(Properties.Resources.page_gear);
        static readonly private Bitmap bitmap_application = new Bitmap(Properties.Resources.application);
        static readonly private Bitmap bitmap_font = new Bitmap(Properties.Resources.font);
        static readonly private Bitmap bitmap_brick = new Bitmap(Properties.Resources.brick);
        static readonly private Bitmap bitmap_shape_3d = new Bitmap(Properties.Resources.shape_3d);
        static readonly private Bitmap bitmap_pictures = new Bitmap(Properties.Resources.pictures);
        static readonly private Bitmap bitmap_border_all = new Bitmap(Properties.Resources.border_all);
        static readonly private Bitmap bitmap_page_code = new Bitmap(Properties.Resources.page_code);
        static readonly private Bitmap bitmap_page_white = new Bitmap(Properties.Resources.page_white);

        public static Bitmap GetIconByFileName(string path)
        {
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".adp" => bitmap_page_white_music,
                ".ats" => bitmap_page_white_text,
                ".cbx" => bitmap_page_white_music,
                ".cfg" => bitmap_page_white_text,
                ".cmo" => bitmap_picture,
                ".csv" => bitmap_page_white_database,
                ".dat" => bitmap_box,
                ".dds" => bitmap_picture,
                ".dll" => bitmap_page_gear,
                ".exe" => bitmap_application,
                ".ft2" => bitmap_font,
                ".fpk" => bitmap_brick,
                ".ghg" => bitmap_shape_3d,
                ".git" => bitmap_page_white_text,
                ".gsc" => bitmap_shape_3d,
                ".ini" => bitmap_page_white_text,
                ".lua" => bitmap_page_white_text,
                ".mp3" => bitmap_page_white_text,
                ".nxg_textures" => bitmap_pictures,
                ".ogg" => bitmap_page_white_music,
                ".pc_shaders" => bitmap_border_all,
                ".scp" => bitmap_page_white_text,
                ".sf" => bitmap_page_white_text,
                ".sub" => bitmap_page_white_text,
                ".subopt" => bitmap_page_white_text,
                ".tex" => bitmap_picture,
                ".tsh" => bitmap_border_all,
                ".txt" => bitmap_page_white_text,
                ".pac" => bitmap_brick,
                ".pak" => bitmap_brick,
                ".wav" => bitmap_page_white_music,
                ".xml" => bitmap_page_code,

                _ => bitmap_page_white,
            };
        }

        public static void OpenFileInternal(string gameFolderPath, string path, byte[] fileBuffer = null, object archiveFile = null)
        {
            switch (Path.GetExtension(path).ToLowerInvariant())
            {
                case ".dat":
                    {
                        new DATForm(gameFolderPath, path).ShowDialog();
                        break;
                    }

                case ".fpk":
                case ".pac":
                case ".pak":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        new PAKForm(path, fileBuffer).ShowDialog();
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
                        fileBuffer ??= File.ReadAllBytes(path);

                        new TextForm(Path.GetFileName(path), fileBuffer, archiveFile).ShowDialog();
                        break;
                    }

                case ".cmo":
                case ".dds":
                case ".tex":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        new ImageForm(path, fileBuffer, ImageFormType.DDS).ShowDialog();
                        break;
                    }

                case ".nxg_textures":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        new ImageForm(path, fileBuffer, ImageFormType.NXGTextures).ShowDialog();
                        break;
                    }

                case ".ft2":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        new FontForm(path, fileBuffer).ShowDialog();
                        break;
                    }

                case ".tsh":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        new TSHForm(path, fileBuffer).ShowDialog();
                        break;
                    }

                case ".adp":
                case ".cbx":
                case ".mp3":
                case ".ogg":
                case ".wav":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        new SoundForm(path, fileBuffer).ShowDialog();
                        break;
                    }

                case ".pc_shaders":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        new PCShadersForm(path, fileBuffer).ShowDialog();
                        break;
                    }

                case ".ghg":
                case ".gsc":
                    {
                        fileBuffer ??= File.ReadAllBytes(path);

                        new ModelForm(path, fileBuffer).ShowDialog();
                        break;
                    }
            }
        }
    }
}
