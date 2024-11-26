using System.Text;

namespace TTGamesExplorerRebirthLib.Helper
{
    public static class Helper
    {
        public static string ToConvertedString(this long value)
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(value), 0, 8);
        }

        public static string ToConvertedString(this ulong value)
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(value), 0, 8);
        }

        public static string ToConvertedString(this int value)
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(value), 0, 4);
        }

        public static string ToConvertedString(this uint value)
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(value), 0, 4);
        }

        public static string ToConvertedString(this short value)
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(value), 0, 2);
        }

        public static string ToConvertedString(this ushort value)
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(value), 0, 2);
        }

        public static string GetExtensionByMagic(string magic)
        {
            if (magic.Contains("DDS"))
            {
                return ".dds";
            }
            else if (magic.Contains("GNF"))
            {
                return ".gnf";
            }
            else if (magic.Contains("BMF"))
            {
                return ".fnt";
            }
            else if (magic.Contains("DDS"))
            {
                return ".dds";
            }

            return magic switch
            {
                "RIFF" => ".wav",
                "DXBC" => ".dxc_pc",
                _ => ".unk",
            };
        }
    }
}