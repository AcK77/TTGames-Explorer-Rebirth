using System.Text;

namespace TTGamesExplorerRebirthLib.Helper
{
    public static class BinaryWriterEx
    {
        public static void WriteStringWithoutPrefixedSize(this BinaryWriter writer, string value)
        {
            writer.Write(Encoding.ASCII.GetBytes(value));
        }

        public static void WriteStringWithoutPrefixedSizeNullTerminated(this BinaryWriter writer, string value)
        {
            writer.Write(Encoding.ASCII.GetBytes(value + '\0'));
        }
    }
}