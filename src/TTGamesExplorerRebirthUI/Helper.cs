using TTGamesExplorerRebirthUI.Forms;
using static TTGamesExplorerRebirthUI.Natives;

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

        static readonly private Bitmap _bitmapPageWhiteMusic = new Bitmap(Properties.Resources.page_white_music);
        static readonly private Bitmap _bitmapPageWhiteText = new Bitmap(Properties.Resources.page_white_text);
        static readonly private Bitmap _bitmapPicture = new Bitmap(Properties.Resources.picture);
        static readonly private Bitmap _bitmapPictures = new Bitmap(Properties.Resources.pictures);
        static readonly private Bitmap _bitmapPageWhiteDatabase = new Bitmap(Properties.Resources.page_white_database);
        static readonly private Bitmap _bitmapBox = new Bitmap(Properties.Resources.box);
        static readonly private Bitmap _bitmapPageGear = new Bitmap(Properties.Resources.page_gear);
        static readonly private Bitmap _bitmapApplication = new Bitmap(Properties.Resources.application);
        static readonly private Bitmap _bitmapFont = new Bitmap(Properties.Resources.font);
        static readonly private Bitmap _bitmapBrick = new Bitmap(Properties.Resources.brick);
        static readonly private Bitmap _bitmapShape3d = new Bitmap(Properties.Resources.shape_3d);
        static readonly private Bitmap _bitmapBorderAll = new Bitmap(Properties.Resources.border_all);
        static readonly private Bitmap _bitmapPageCode = new Bitmap(Properties.Resources.page_code);
        static readonly private Bitmap _bitmapPageWhite = new Bitmap(Properties.Resources.page_white);

        public static Bitmap GetIconByFileName(string path)
        {
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".adp" => _bitmapPageWhiteMusic,
                ".ats" => _bitmapPageWhiteText,
                ".cbx" => _bitmapPageWhiteMusic,
                ".cfg" => _bitmapPageWhiteText,
                ".cmo" => _bitmapPicture,
                ".csv" => _bitmapPageWhiteDatabase,
                ".dat" => _bitmapBox,
                ".dds" => _bitmapPicture,
                ".dll" => _bitmapPageGear,
                ".exe" => _bitmapApplication,
                ".ft2" => _bitmapFont,
                ".fpk" => _bitmapBrick,
                ".ghg" => _bitmapShape3d,
                ".git" => _bitmapPageWhiteText,
                ".gsc" => _bitmapShape3d,
                ".ini" => _bitmapPageWhiteText,
                ".lua" => _bitmapPageWhiteText,
                ".mp3" => _bitmapPageWhiteText,
                ".nxg_textures" => _bitmapPictures,
                ".ogg" => _bitmapPageWhiteMusic,
                ".pc_shaders" => _bitmapBorderAll,
                ".blob" => _bitmapBorderAll,
                ".scp" => _bitmapPageWhiteText,
                ".sf" => _bitmapPageWhiteText,
                ".sub" => _bitmapPageWhiteText,
                ".subopt" => _bitmapPageWhiteText,
                ".tex" => _bitmapPicture,
                ".tsh" => _bitmapBorderAll,
                ".txt" => _bitmapPageWhiteText,
                ".pac" => _bitmapBrick,
                ".pak" => _bitmapBrick,
                ".wav" => _bitmapPageWhiteMusic,
                ".xml" => _bitmapPageCode,

                _ => _bitmapPageWhite,
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

                case ".blob":
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
