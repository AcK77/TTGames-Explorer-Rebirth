using System.Text;

namespace TTGamesExplorerRebirthLib.Hashes
{
    public static class Fnv
    {
        public static uint Fnv_32(string text, bool alternate = false)
        {
            return Fnv_32(Encoding.ASCII.GetBytes(text), alternate);
        }

        public static uint Fnv_32(byte[] buffer, bool alternate = false)
        {
            uint prime = 0x1000193;
            uint hash  = 0;

            foreach (byte b in buffer)
            {
                hash = alternate ? (hash ^ b) * prime : (hash * prime) ^ b;
            }

            return hash;
        }

        public static uint Fnv1a_32_TTGames(string text)
        {
            uint prime = 0x199933;
            uint hash  = 0x811C9DC5;

            foreach (byte b in Encoding.ASCII.GetBytes(text.ToUpperInvariant()))
            {
                hash = (hash ^ b) * prime;
            }

            return hash;
        }

        public static uint Fnv1_32_PKWin(string text)
        {
            return Fnv1_32($"./{text.ToLowerInvariant()}");
        }

        public static uint Fnv1_32(string text, bool alternate = false)
        {
            return Fnv1_32(Encoding.ASCII.GetBytes(text), alternate);
        }

        public static uint Fnv1_32(byte[] buffer, bool alternate = false)
        {
            uint prime = 0x1000193;
            uint hash  = 0x811C9DC5;

            foreach (byte b in buffer)
            {
                hash = alternate ? (hash ^ b) * prime : (hash * prime) ^ b;
            }

            return hash;
        }

        public static ulong Fnv1_64(byte[] buffer, bool alternate = false)
        {
            ulong prime = 0x100000001B3;
            ulong hash  = 0xCBF29CE484222325;

            foreach (byte b in buffer)
            {
                hash = alternate ? (hash ^ b) * prime : (hash * prime) ^ b;
            }

            return hash;
        }

        public static byte[] Fnv1_128(byte[] buffer, bool alternate = false)
        {
            uint primeB = 0x01000000;
            uint primeD = 0x0000013B;
            uint primeBD = 0xFFFEC5;

            ulong baseH = 0x6c62272e07bb0142;
            ulong baseL = 0x62b821756295c58d;

            ulong a = baseH >> 32, b = (uint)baseH, c = baseL >> 32, d = (uint)baseL;

            ulong f = 0, fLm = 0;
            int i = 0;
            unchecked
            {
                for (; i < buffer.Length; ++i)
                {
                    if (!alternate)
                    {
                        d ^= buffer[i];
                    }

                    // Below is an optimized implementation (limited) of the LX4Cnh algorithm specially for Fnv1a128
                    // (c) Denis Kuzmin <x-3F@outlook.com> github/3F

                    f = b * primeB;

                    ulong v = (uint)f;

                    f = (f >> 32) + v;

                    if (a > b)
                    {
                        f += (uint)((a - b) * primeB);
                    }
                    else if (a < b)
                    {
                        f -= (uint)((b - a) * primeB);
                    }

                    ulong fHigh = (f << 32) + (uint)v;
                    ulong r2 = d * primeD;

                    v = (r2 >> 32) + (r2 & 0xFFF_FFFF_FFFF_FFFF);

                    f = (r2 & 0xF000_0000_0000_0000) >> 32;

                    if (c > d)
                    {
                        fLm = v;
                        v += (c - d) * primeD;
                        if (fLm > v) f += 0x100000000;
                    }
                    else if (c < d)
                    {
                        fLm = v;
                        v -= (d - c) * primeD;
                        if (fLm < v) f -= 0x100000000;
                    }

                    fLm = (((ulong)(uint)v) << 32) + (uint)r2;

                    f = fHigh + fLm + f + (v >> 32);

                    fHigh = (a << 32) + b; // fa
                    v = (c << 32) + d; // fb

                    if (fHigh < v)
                    {
                        f += (v - fHigh) * primeBD;
                    }
                    else if (fHigh > v)
                    {
                        f -= (fHigh - v) * primeBD;
                    }

                    a = f >> 32;
                    b = (uint)f;
                    c = fLm >> 32;
                    d = (uint)fLm;

                    if (alternate)
                    {
                        d ^= buffer[i];
                    }
                }
            }

            ulong low = 0;

            if (i < 1)
            {
                low = baseL;

                f = baseH;
            }

            low = fLm;

            return [.. BitConverter.GetBytes(f).Reverse(), .. BitConverter.GetBytes(low).Reverse()];
        }
    }
}