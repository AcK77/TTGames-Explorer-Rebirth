namespace TTGamesExplorerRebirthLib.Compression
{
    public static class UnRefpack
    {
        /// <summary>
        ///     Give source byte buffer and return destination byte buffer, process to Refpack decompression.
        ///     This algorithm is mostly used by TTFusion.
        /// </summary>
        /// <remarks>
        ///     Based from RE of LEGO Ninjago Tournament by Ac_K.
        ///     Based on fibdec by yukinogatari:
        ///     https://github.com/yukinogatari/Reverse-Engineering
        /// </remarks>
        /// <param name="srcBuffer">
        ///     Byte array representing the source data to be decompressed.
        /// </param>
        /// <returns>
        ///     Byte array representing the decompressed destination data.
        /// </returns>
        public static byte[] Decompress(byte[] srcBuffer)
        {
            List<byte> dstBuffer = [];
            int        position  = 0;
            int        readCount = 0;

            while (position < srcBuffer.Length)
            {
                byte b = srcBuffer[position];
                position += 1;

                if (b >= 0xFC)
                {
                    readCount = b - 0xFC;

                    for (int i = position; i < position + readCount; i++)
                    {
                        dstBuffer.Add(srcBuffer[i]);
                    }

                    position += readCount;

                    break;
                }

                // Raw data.
                // 111xxxxx
                // Count -> (x + 1) * 4
                if (b >= 0xE0)
                {
                    readCount = ((b & 0b11111) + 1) * 4;

                    for (int i = position; i < position + readCount; i++)
                    {
                        dstBuffer.Add(srcBuffer[i]);
                    }

                    position += readCount;
                }
                else // Raw data, then copy from the output buffer.
                {
                    int rawCount = 0;
                    int offset   = 0;

                    // 110xxyyz zzzzzzzz zzzzzzzz yyyyyyyy
                    // Raw count  -> x
                    // Read count -> y + 5
                    // Offset     -> z + 1
                    if (b >= 0xC0)
                    {
                        byte b2 = srcBuffer[position];
                        byte b3 = srcBuffer[position + 1];
                        byte b4 = srcBuffer[position + 2];

                        position += 3;

                        rawCount  = (b & 0b00011000) >> 3;
                        readCount = ((b & 0b00000110) << 7) + b4 + 5;
                        offset    = ((b & 1) << 0b00000001) + (b2 << 8) + b3 + 1;
                    }
                    // 10yyyyyy xxzzzzzz zzzzzzzz
                    // Raw count  -> x
                    // Read count -> y + 4
                    // Offset     -> z + 1
                    else if (b >= 0x80)
                    {
                        byte b2 = srcBuffer[position];
                        byte b3 = srcBuffer[position + 1];

                        position += 2;

                        rawCount  = b2 >> 6;
                        readCount = (b & 0b00111111) + 4;
                        offset    = ((b2 & 0b00111111) << 8) + b3 + 1;
                    }
                    // 0yyyxxzz zzzzzzzz
                    // Raw count  -> x
                    // Read count -> y + 3
                    // Offset     -> z + 1
                    else
                    {
                        byte b2 = srcBuffer[position];

                        position += 1;

                        rawCount  = (b & 0b00001100) >> 2;
                        readCount = ((b & 0b01110000) >> 4) + 3;
                        offset    = ((b & 0b00000011) << 8) + b2 + 1;
                    }

                    for (int i = position; i < position + rawCount; i++)
                    {
                        dstBuffer.Add(srcBuffer[i]);
                    }

                    position += rawCount;

                    for (int i = 0; i < readCount; i++)
                    {
                        dstBuffer.Add(dstBuffer.ToArray()[dstBuffer.Count - offset]);
                    }
                }
            }

            return [.. dstBuffer];
        }
    }
}
