using System.Buffers.Binary;
using System.Text;

namespace TTGamesExplorerRebirthLib.Helper
{
    public static class BinaryReaderEx
    {
        public static string ReadUInt64AsString(this BinaryReader reader)
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(reader.ReadUInt64()), 0, 8);
        }

        public static string ReadUInt32AsString(this BinaryReader reader)
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(reader.ReadUInt32()), 0, 4);
        }

        public static double ReadDoubleBigEndian(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadDoubleBigEndian(reader.ReadBytes(8));
        }

        public static short ReadInt16BigEndian(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadInt16BigEndian(reader.ReadBytes(2));
        }

        public static int ReadInt32BigEndian(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
        }

        public static long ReadInt64BigEndian(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadInt64BigEndian(reader.ReadBytes(8));
        }

        public static float ReadSingleBigEndian(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadSingleBigEndian(reader.ReadBytes(4));
        }

        public static ushort ReadUInt16BigEndian(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(2));
        }

        public static uint ReadUInt32BigEndian(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadUInt32BigEndian(reader.ReadBytes(4));
        }

        public static ulong ReadUInt64BigEndian(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadUInt64BigEndian(reader.ReadBytes(8));
        }

        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            List<byte> strBytes = [];

            int b;

            while ((b = reader.BaseStream.ReadByte()) != 0x00)
            {
                strBytes.Add((byte)b);
            }

            return Encoding.ASCII.GetString(strBytes.ToArray());
        }

        public static string ReadSized8NullTerminatedString(this BinaryReader reader)
        {
            byte size = reader.ReadByte();

            List<byte> strBytes = [];

            int b;

            while ((b = reader.BaseStream.ReadByte()) != 0x00)
            {
                strBytes.Add((byte)b);
            }

            string text = Encoding.ASCII.GetString(strBytes.ToArray());

            if (text.Length != size - 1)
            {
                throw new InvalidDataException();
            }

            return text;
        }

        public static string ReadSized16NullTerminatedString(this BinaryReader reader)
        {
            ushort size = reader.ReadUInt16BigEndian();

            List<byte> strBytes = [];

            int b;

            while ((b = reader.BaseStream.ReadByte()) != 0x00)
            {
                strBytes.Add((byte)b);
            }

            string text = Encoding.ASCII.GetString(strBytes.ToArray());

            if (text.Length != size - 1)
            {
                throw new InvalidDataException();
            }

            return text;
        }

        public static string ReadSized32NullTerminatedString(this BinaryReader reader)
        {
            uint size = reader.ReadUInt32BigEndian();

            List<byte> strBytes = [];

            int b;

            while ((b = reader.BaseStream.ReadByte()) != 0x00)
            {
                strBytes.Add((byte)b);
            }

            string text = Encoding.ASCII.GetString(strBytes.ToArray());

            if (text.Length != size - 1)
            {
                throw new InvalidDataException();
            }

            return text;
        }

        public static string ReadSizedString(this BinaryReader reader, uint size)
        {
            return Encoding.ASCII.GetString(reader.ReadBytes((int)size));
        }
    }
}