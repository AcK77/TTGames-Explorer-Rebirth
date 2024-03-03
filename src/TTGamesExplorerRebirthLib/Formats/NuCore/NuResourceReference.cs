using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuResourceReference
    {
        public NuResourceReference Deserialize(BinaryReader reader, NuResourceHeader nuResourceHeader, uint size)
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

            return this;
        }
    }
#pragma warning restore IDE0059
}