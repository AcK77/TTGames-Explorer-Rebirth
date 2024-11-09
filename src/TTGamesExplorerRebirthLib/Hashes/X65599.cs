namespace TTGamesExplorerRebirthLib.Hashes
{
    public static class HashX65599
    {
        public static int Get(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return 0;
            }

            int hash = 0;
            foreach (byte b in buffer)
            {
                hash = 65599 * hash + b;
            }

            return hash;
        }
    }
}
