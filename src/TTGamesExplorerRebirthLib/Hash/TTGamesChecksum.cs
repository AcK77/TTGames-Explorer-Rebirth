namespace TTGamesExplorerRebirthLib.Hash
{
    public static class TTGamesChecksum
    {
        public static int PAK(byte[] buffer)
        {
            int checksum = 0x12345678;

            foreach (byte b in buffer)
            {
                checksum += b;
            }

            return checksum;
        }
    }
}