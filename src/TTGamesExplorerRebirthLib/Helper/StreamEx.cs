using System.Text;

namespace TTGamesExplorerRebirthLib.Helper
{
    public static class StreamEx
    {
        public static string ReadNullTerminatedString(this Stream Stream)
        {
            List<byte> strBytes = [];

            int b;

            while ((b = Stream.ReadByte()) != 0x00)
            {
                strBytes.Add((byte)b);
            }

            return Encoding.ASCII.GetString(strBytes.ToArray());
        }
    }
}