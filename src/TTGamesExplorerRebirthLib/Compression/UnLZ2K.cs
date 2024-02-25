namespace TTGamesExplorerRebirthLib.Compression
{
    public static class UnLZ2K
    {
        private const uint ChunkSizeConst = 0x2000;

        private static byte[]  _srcBuffer;
        private static byte[]  _chunkBuffer;
        private static byte[]  _smallByteBuffer;
        private static byte[]  _largeByteBuffer;
        private static short[] _smallWordBuffer;
        private static short[] _largeWordBuffer;
        private static short[] _parallelBuffer0;
        private static short[] _parallelBuffer1;

        private static uint  _bitStream;
        private static byte  _previousBitAlign;
        private static byte  _lastByteRead;
        private static uint  _srcOffset;
        private static int   _literalsToCopy;
        private static short _chunksWithCurrentSetup;
        private static uint  _readOffset;

        /// <summary>
        ///     Give source byte buffer and destination byte buffer, process to LZ2K decompression.
        ///     This algorithm is mostly used by TTGames.
        /// </summary>
        /// <remarks>
        ///     Based from RE of Lego Worlds by Ac_K.
        ///     Based on unlz2K by Trevor Natiuk:
        ///     https://github.com/pianistrevor/unlz2k
        /// </remarks>
        /// <param name="srcBuffer">
        ///     Byte array representing the source data to be decompressed.
        /// </param>
        /// <param name="dstBuffer">
        ///     Byte array representing the decompressed destination data.
        /// </param>
        public static void Decompress(byte[] srcBuffer, byte[] dstBuffer)
        {
            if (dstBuffer.Length == 0)
            {
                return;
            }

            _srcBuffer       = srcBuffer;
            _chunkBuffer     = new byte[ChunkSizeConst];
            _smallByteBuffer = new byte[20];
            _largeByteBuffer = new byte[510];
            _smallWordBuffer = new short[256];
            _largeWordBuffer = new short[4096];
            _parallelBuffer0 = new short[1024];
            _parallelBuffer1 = new short[1024];

            _bitStream              = 0;
            _previousBitAlign       = 0;
            _lastByteRead           = 0;
            _srcOffset              = 0;
            _literalsToCopy         = 0;
            _chunksWithCurrentSetup = 0;
            _readOffset             = 0;

            uint bytesLeft = (uint)dstBuffer.Length;

            using MemoryStream dstStream = new(dstBuffer);
            using BinaryWriter dstWriter = new(dstStream);

            LoadIntoBitstream(0x20);

            while (bytesLeft != 0)
            {
                uint chunkSize = bytesLeft < ChunkSizeConst ? bytesLeft : ChunkSizeConst;

                DecodeChunk(chunkSize);

                byte[] tempBuffer = new byte[chunkSize];

                Array.Copy(_chunkBuffer, tempBuffer, chunkSize);

                dstWriter.Write(tempBuffer);

                bytesLeft -= chunkSize;
            }

            dstBuffer = dstStream.ToArray();
        }

        private static void LoadIntoBitstream(byte bits)
        {
            _bitStream <<= bits;

            if (bits > _previousBitAlign)
            {
                do
                {
                    bits             -= _previousBitAlign;
                    _bitStream       |= (uint)_lastByteRead << bits;
                    _lastByteRead     = (byte)((_srcOffset == (uint)_srcBuffer.Length) ? 0 : _srcBuffer[_srcOffset++]);
                    _previousBitAlign = 8;
                } while (bits > _previousBitAlign);
            }

            _previousBitAlign -= bits;
            _bitStream        |= (uint)_lastByteRead >> _previousBitAlign;
            _bitStream &= 0xFFFFFFFF;
        }

        private static void DecodeChunk(uint chunkSize)
        {
            uint dstOffset = 0;

            --_literalsToCopy;
            if (_literalsToCopy >= 0)
            {
                while (_literalsToCopy >= 0)
                {
                    _chunkBuffer[dstOffset++] = _chunkBuffer[_readOffset++];
                    _readOffset &= 0x1FFF;

                    if (dstOffset == chunkSize)
                    {
                        return;
                    }

                    --_literalsToCopy;
                }
            }

            while (dstOffset < chunkSize)
            {
                short decodedBitStream = DecodeBitStream();
                if (decodedBitStream <= 255)
                {
                    _chunkBuffer[dstOffset++] = (byte)decodedBitStream;

                    if (dstOffset == chunkSize)
                    {
                        return;
                    }
                }
                else
                {
                    _literalsToCopy = DecodeBitStreamForLiterals();
                    _readOffset     = (uint)(dstOffset - _literalsToCopy - 1) & 0x1FFF;
                    _literalsToCopy = decodedBitStream - 254;

                    while (_literalsToCopy >= 0)
                    {
                        _chunkBuffer[dstOffset++] = _chunkBuffer[_readOffset++];
                        _readOffset &= 0x1FFF;

                        if (dstOffset == chunkSize)
                        {
                            return;
                        }

                        --_literalsToCopy;
                    }
                }
            }

            if (dstOffset > chunkSize)
            {
                throw new IndexOutOfRangeException();
            }
        }

        private static short DecodeBitStream()
        {
            if (_chunksWithCurrentSetup == 0)
            {
                _chunksWithCurrentSetup = (short)(_bitStream >> 16);

                LoadIntoBitstream(0x10);
                FillSmallDicts(19, 5, 3);
                FillLargeDicts();
                FillSmallDicts(14, 4, -1);
            }

            _chunksWithCurrentSetup--;

            short tmpValue = _largeWordBuffer[_bitStream >> 20];
            if (tmpValue >= _largeByteBuffer.Length)
            {
                uint mask = 0x80000;

                while (tmpValue >= _largeByteBuffer.Length)
                {
                    tmpValue = ((_bitStream & mask) == 0) ? _parallelBuffer0[tmpValue] : _parallelBuffer1[tmpValue];
                    mask >>= 1;
                }
            }

            LoadIntoBitstream(_largeByteBuffer[tmpValue]);

            return tmpValue;
        }

        private static int DecodeBitStreamForLiterals()
        {
            short tmpValue = _smallWordBuffer[_bitStream >> 24];
            if (tmpValue >= 14)
            {
                uint mask = 0x800000;

                while (tmpValue >= 14)
                {
                    tmpValue = ((_bitStream & mask) == 0) ? _parallelBuffer0[tmpValue] : _parallelBuffer1[tmpValue];
                    mask >>= 1;
                }
            }

            LoadIntoBitstream(_smallByteBuffer[tmpValue]);

            if (tmpValue == 0)
            {
                return 0;
            }

            if (tmpValue == 1)
            {
                return 2;
            }

            tmpValue--;

            uint tmpBitStream = _bitStream >> (32 - tmpValue);

            LoadIntoBitstream((byte)tmpValue);

            return (int)(tmpBitStream + (1 << tmpValue));
        }

        private static void FillSmallDicts(byte length, byte bits, sbyte specialInd)
        {
            byte tmpValue1 = (byte)(_bitStream >> (32 - bits)); // NOTE: bits is never 0.

            LoadIntoBitstream(bits);

            if (tmpValue1 != 0)
            {
                byte tmpValue2 = 0;

                if (tmpValue1 > 0)
                {
                    while (tmpValue2 < tmpValue1)
                    {
                        uint tmpBitStream = (byte)(_bitStream >> 29);
                        bits = 3;

                        if (tmpBitStream == 7)
                        {
                            uint mask = 0x10000000;

                            if ((_bitStream & mask) == 0)
                            {
                                bits = 4;
                            }
                            else
                            {
                                byte counter = 0;

                                while ((_bitStream & mask) != 0)
                                {
                                    mask >>= 1;
                                    counter += 1;
                                }

                                bits          = (byte)(counter + 4);
                                tmpBitStream += counter;
                            }
                        }

                        LoadIntoBitstream(bits);

                        _smallByteBuffer[tmpValue2] = (byte)tmpBitStream;

                        tmpValue2++;

                        if (tmpValue2 == specialInd)
                        {
                            sbyte specialLength = (sbyte)(_bitStream >> 30);

                            LoadIntoBitstream(2);

                            if (specialLength >= 1)
                            {
                                Array.Clear(_smallByteBuffer, tmpValue2, specialLength);

                                tmpValue2 += (byte)specialLength;
                            }
                        }
                    }
                }

                if (tmpValue2 < length)
                {
                    Array.Clear(_smallByteBuffer, tmpValue2, length - tmpValue2);
                }

                FillWordsUsingBytes(length, _smallByteBuffer, 8, _smallWordBuffer);

                return;
            }

            tmpValue1 = (byte)(_bitStream >> (32 - bits));

            LoadIntoBitstream(bits);

            if (length > 0)
            {
                Array.Clear(_smallByteBuffer, 0, length);
            }

            for (int i = 0; i < _smallWordBuffer.Length; i++)
            {
                _smallWordBuffer[i] = tmpValue1;
            }
        }

        private static void FillLargeDicts()
        {
            short tmpValue1 = (short)(_bitStream >> 23);

            LoadIntoBitstream(9);

            if (tmpValue1 == 0)
            {
                short tmpValue2 = (short)(_bitStream >> 23);

                LoadIntoBitstream(9);

                Array.Clear(_largeByteBuffer, 0, _largeByteBuffer.Length);

                for (int i = 0; i < _largeWordBuffer.Length; i++)
                {
                    _largeWordBuffer[i] = tmpValue2;
                }

                return;
            }

            ushort bytes = 0;
            if (tmpValue1 < 0)
            {
                // NOTE: Does this ever happen?

                Array.Clear(_largeByteBuffer, 0, _largeByteBuffer.Length);

                FillWordsUsingBytes((short)_largeByteBuffer.Length, _largeByteBuffer, 12, _largeWordBuffer);

                return;
            }

            while (bytes < tmpValue1)
            {
                ushort tmpLength = (ushort)(_bitStream >> 24);
                short  tmpValue2 = _smallWordBuffer[tmpLength];

                if (tmpValue2 >= 19)
                {
                    uint mask = 0x800000;

                    do
                    {
                        tmpValue2 = ((_bitStream & mask) == 0) ? _parallelBuffer0[tmpValue2] : _parallelBuffer1[tmpValue2];
                        mask >>= 1;
                    } while (tmpValue2 >= 19);
                }

                LoadIntoBitstream(_smallByteBuffer[tmpValue2]);

                if (tmpValue2 > 2)
                {
                    tmpValue2 -= 2;
                    _largeByteBuffer[bytes++] = (byte)tmpValue2;
                }
                else
                {
                    if (tmpValue2 == 0)
                    {
                        tmpLength = 1;
                    }
                    else if (tmpValue2 == 1)
                    {
                        tmpValue2 = (short)(_bitStream >> 28);

                        LoadIntoBitstream(4);

                        tmpLength = (ushort)(tmpValue2 + 3);
                    }
                    else
                    {
                        tmpValue2 = (short)(_bitStream >> 23);

                        LoadIntoBitstream(9);

                        tmpLength = (ushort)(tmpValue2 + 20);
                    }

                    if (tmpLength > 0)
                    {
                        Array.Clear(_largeByteBuffer, bytes, tmpLength);

                        bytes += tmpLength;
                    }
                }
            }

            if (bytes < _largeByteBuffer.Length)
            {
                Array.Clear(_largeByteBuffer, bytes, _largeByteBuffer.Length - bytes);
            }

            FillWordsUsingBytes((short)_largeByteBuffer.Length, _largeByteBuffer, 12, _largeWordBuffer);

            return;
        }

        private static void FillWordsUsingBytes(short bytesLength, byte[] bytesBuffer, short pivot, short[] wordsBuffer)
        {
            ushort[] srcBuffer  = new ushort[17];
            ushort[] destBuffer = new ushort[18];

            for (int i = 0; i < bytesLength; i++)
            {
                srcBuffer[bytesBuffer[i]]++;
            }

            sbyte shift = 14;
            sbyte ind = 1;
            ushort low, high;

            while (ind <= 16)
            {
                low    = srcBuffer[ind];
                high   = srcBuffer[ind + 1];
                low  <<= shift + 1;
                high <<= shift;
                low   += destBuffer[ind];
                ind   += 4;
                high  += low;
                high  &= 0xFFFF;

                destBuffer[ind - 3] = low;
                destBuffer[ind - 2] = high;

                low   = (ushort)(srcBuffer[ind - 2] << (shift - 1));
                low  += high;
                high  = (ushort)(srcBuffer[ind - 1] << (shift - 2));
                high += low;

                destBuffer[ind - 1] = low;
                destBuffer[ind] = high;

                shift -= 4;
            }

            if (destBuffer[17] != 0)
            {
                throw new Exception("Wrong table.");
            }

            shift = (sbyte)(pivot - 1);

            int tmpValue     = 16 - pivot;
            int tmpValueCopy = tmpValue;

            for (int i = 1; i <= pivot; ++i)
            {
                destBuffer[i] >>= tmpValue;
                srcBuffer[i]    = (ushort)(1 << shift--);
            }

            tmpValue--;

            for (int i = pivot + 1; i <= 16; ++i)
            {
                srcBuffer[i] = (ushort)(1 << tmpValue--);
            }

            ushort comp1 = destBuffer[pivot + 1];
            comp1 >>= 16 - pivot;

            if (comp1 != 0)
            {
                ushort comp2 = (ushort)(1 << pivot);

                if (comp1 != comp2)
                {
                    for (int i = comp1; i < comp2; i++)
                    {
                        wordsBuffer[i] = 0;
                    }
                }
            }

            if (bytesLength <= 0)
            {
                return;
            }

            shift = (sbyte)(15 - pivot);

            ushort mask      = (ushort)(1 << shift);
            short  tmpValue2 = bytesLength;

            for (int i = 0; i < bytesLength; i++)
            {
                byte tmpByte = bytesBuffer[i];
                if (tmpByte != 0)
                {
                    ushort destValue = destBuffer[tmpByte];
                    ushort srcValue  = srcBuffer[tmpByte];

                    srcValue += destValue;

                    if (tmpByte > pivot)
                    {
                        short[] tmpBuffer = wordsBuffer;
                        short   tmpOffset = (short)(destValue >> tmpValueCopy);
                        byte    newLength = (byte)(tmpByte - pivot);

                        if (newLength != 0)
                        {
                            while (newLength != 0)
                            {
                                if (tmpBuffer[tmpOffset] == 0)
                                {
                                    _parallelBuffer0[tmpValue2] = 0;
                                    _parallelBuffer1[tmpValue2] = 0;

                                    tmpBuffer[tmpOffset] = tmpValue2++;
                                }

                                tmpOffset = tmpBuffer[tmpOffset];
                                tmpBuffer = ((destValue & mask) == 0) ? _parallelBuffer0 : _parallelBuffer1;
                                destValue += destValue;
                                newLength--;
                            }
                        }

                        tmpBuffer[tmpOffset] = (short)i;
                    }
                    else if (destValue < srcValue)
                    {
                        for (int j = destValue; j < srcValue; j++)
                        {
                            wordsBuffer[j] = (short)i;
                        }
                    }

                    destBuffer[tmpByte] = srcValue;
                }
            }
        }
    }
}