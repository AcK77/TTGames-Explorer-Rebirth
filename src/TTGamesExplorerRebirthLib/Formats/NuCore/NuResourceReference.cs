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
                uint id   = reader.ReadUInt32BigEndian();
                uint type = reader.ReadUInt32BigEndian();

                if (nuResourceHeader.Version <= 6)
                {
                    uint oldParam = reader.ReadUInt32BigEndian();
                }

                uint unknown1 = reader.ReadUInt32BigEndian();
                uint fnv1aHash = reader.ReadUInt32BigEndian();

                if (nuResourceHeader.Version >= 3)
                {
                    uint platformsAndClasses = reader.ReadUInt32BigEndian();

                    if (nuResourceHeader.Version >= 6)
                    {
                        uint forContext  = reader.ReadUInt32BigEndian();
                        uint withContext = reader.ReadUInt32BigEndian();
                    }
                }

                if (nuResourceHeader.Version >= 17)
                {
                    byte[] nuChecksum = reader.ReadBytes(0x10);
                    uint   unknown2   = reader.ReadUInt32BigEndian();
                    byte[] unknown3   = reader.ReadBytes(0x10);
                }
            }

            if (nuResourceHeader.Version >= 8)
            {
                uint discipline = reader.ReadUInt32BigEndian();
            }

            return this;
        }
    }
#pragma warning restore IDE0059
}