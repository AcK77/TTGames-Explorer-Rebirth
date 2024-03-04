using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public class NuFileHeader
    {
        public static string Magic = ".CC4";

        public uint Deserialize(BinaryReader reader)
        {
            uint nuResourceHeaderSize = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            return nuResourceHeaderSize;
        }

        public bool IsNuFile(BinaryReader reader)
        {
            long position = reader.BaseStream.Position;

            reader.ReadUInt32();

            if (reader.ReadUInt32AsString() != Magic)
            {
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                return false;
            }

            reader.BaseStream.Seek(position, SeekOrigin.Begin);

            return true;
        }
    }
}