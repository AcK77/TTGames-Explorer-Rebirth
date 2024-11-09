namespace TTGamesExplorerRebirthLib.Hashes
{
    public static class Crc32
    {
        private static uint[] _checksumTable => GenerateCrc32Table();

        public static uint[] GenerateCrc32Table()
        {
            return Enumerable.Range(0, 256).Select(i =>
            {
                uint tableEntry = (uint)i;

                for (int j = 0; j < 8; ++j)
                {
                    tableEntry = ((tableEntry & 1) != 0) ? (0xEDB88320 ^ (tableEntry >> 1)) : (tableEntry >> 1);
                }

                return tableEntry;
            }).ToArray();
        }

        public static uint Get<T>(IEnumerable<T> buffer)
        {
            return ~buffer.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) => _checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8));
        }

        public static uint GetTTFusion<T>(IEnumerable<T> buffer)
        {
            return ~Get(buffer);
        }
    }
}
