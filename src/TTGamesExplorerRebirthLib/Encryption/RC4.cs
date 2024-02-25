﻿namespace TTGamesExplorerRebirthLib.Encryption
{
    public static class RC4
    {
        /// <summary>
        ///     Give data and an encryption key, apply RC4 cryptography. RC4 is symmetric,
        ///     which means this single method will work for encrypting and decrypting.
        /// </summary>
        /// <remarks>
        ///     Original implementation by Christopher Whitley:
        ///     https://github.com/manbeardgames/RC4
        ///
        ///     https://en.wikipedia.org/wiki/RC4
        /// </remarks>
        /// <param name="data">
        ///     Byte array representing the data to be encrypted/decrypted.
        /// </param>
        /// <param name="key">
        ///     Byte array representing the key to use.
        /// </param>
        /// <returns>
        ///     Byte array representing the encrypted/decrypted data.
        /// </returns>
        public static byte[] Crypt(byte[] data, byte[] key)
        {
            // Key Scheduling Algorithm Phase:
            // KSA Phase Step 1: First, the entries of S are set equal to the values of 0 to 255 in ascending order.
            int[] s = new int[256];
            for (int _ = 0; _ < 256; _++)
            {
                s[_] = _;
            }

            // KSA Phase Step 2a: Next, a temporary vector T is created.
            int[] t = new int[256];

            // KSA Phase Step 2b: If the length of the key k is 256 bytes, then k is assigned to T.
            if (key.Length == 256)
            {
                Buffer.BlockCopy(key, 0, t, 0, key.Length);
            }
            else
            {
                // Otherwise, for a key with a given length, copy the elements of
                // the key into vector T, repeating for as many times as neccessary to fill T.
                for (int _ = 0; _ < 256; _++)
                {
                    t[_] = key[_ % key.Length];
                }
            }

            // KSA Phase Step 3: We use T to produce the initial permutation of S...
            int i, j = 0;
            for (i = 0; i < 256; i++)
            {
                // Increment j by the sum of S[i] and T[i], however keeping it within the 
                // range of 0 to 255 using mod (%) division.
                j = (j + s[i] + t[i]) % 256;

                //  Swap the values of S[i] and S[j]
                (s[j], s[i]) = (s[i], s[j]);
            }

            // Pseudo random generation algorithm (Stream Generation):
            // Once the vector S is initialized from above in the Key Scheduling Algorithm Phase,
            // the input key is no longer used. In this phase, for the length of the data, we...
            i = j = 0;
            byte[] result = new byte[data.Length];
            for (int iteration = 0; iteration < data.Length; iteration++)
            {
                // PRGA Phase Step 1. Continously increment i from 0 to 255, starting it back
                //                    at 0 once we go beyond 255 (this is done with mod (%) division.
                i = (i + 1) % 256;

                // PRGA Phase Step 2. Lookup the i'th element of S and add it to j, keeping the
                //                    result within the range of 0 to 255 using mod (%) division.
                j = (j + s[i]) % 256;

                // PRGA Phase Step 3. Swap the values of S[i] and S[j].
                (s[j], s[i]) = (s[i], s[j]);

                // PRGA Phase Step 4. Use the result of the sum of S[i] and S[j], mod (%) by 256,
                //                    to get the index of S that handls the value of the stream value K.
                int k = s[(s[i] + s[j]) % 256];

                // PRGA Phase Step 5. Use bitwise exclusive OR (^) with the next byte in the data to
                //                    produce the next byte of the resulting ciphertext (when
                //                    encrypting) or plaintext (when decrypting).
                result[iteration] = Convert.ToByte(data[iteration] ^ k);
            }

            return result;
        }
    }
}