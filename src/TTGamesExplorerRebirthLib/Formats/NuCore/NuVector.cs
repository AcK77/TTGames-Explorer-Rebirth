using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuVector
    {
        public static string Magic = "ROTV";

        public void Deserialize(BinaryReader reader, NuResourceHeader nuResourceHeader)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint size = reader.ReadUInt32BigEndian();
            uint id   = reader.ReadUInt32BigEndian();

            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    uint type = reader.ReadUInt32BigEndian();

                    if (nuResourceHeader.Version <= 6)
                    {
                        uint oldParam = reader.ReadUInt32BigEndian();
                    }

                    ulong hash = reader.ReadUInt32BigEndian();
                    if (hash == 1)
                    {
                        uint   unknown    = reader.ReadUInt32BigEndian();
                        byte[] nuChecksum = reader.ReadBytes(0x10);
                    }
                    else
                    {
                        uint unknown = reader.ReadUInt32BigEndian();
                    }

                    if (nuResourceHeader.Version >= 3)
                    {
                        uint platformsAndClasses = reader.ReadUInt32BigEndian();

                        if (nuResourceHeader.Version >= 6)
                        {
                            uint forContext  = reader.ReadUInt32BigEndian();
                            uint withContext = reader.ReadUInt32BigEndian();
                        }

                        if (nuResourceHeader.Version >= 8)
                        {
                            uint discipline = reader.ReadUInt32BigEndian();
                        }
                    }
                }
            }
        }
    }
#pragma warning restore IDE0059
}
