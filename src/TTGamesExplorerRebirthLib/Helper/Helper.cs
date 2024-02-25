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
    }
}