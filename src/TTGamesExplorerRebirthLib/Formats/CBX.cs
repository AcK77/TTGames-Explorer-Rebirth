using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    /// <summary>
    ///     Give cbx file data, convert it to wav format.
    /// </summary>
    /// <remarks>
    ///     Based on CBXDecoder (unlicensed) by Connor Harrison:
    ///     https://github.com/connorh315/CBXDecoder
    ///     
    ///     Code optimized by Ac_K.
    /// </remarks>
    public class CBX
    {
        private const string MagicBox  = "!B0X";
        private const int    ChunkSize = 0x1B0;

        private readonly float[] _exponentTable = new float[64];

        private readonly float[] _floatTable = [
             0.000000f, -0.996776f, -0.990327f, -0.983879f, -0.977431f, -0.970982f, -0.964534f, -0.958085f,
            -0.951637f, -0.930754f, -0.904960f, -0.879167f, -0.853373f, -0.827579f, -0.801786f, -0.775992f,
            -0.750198f, -0.724405f, -0.698611f, -0.670635f, -0.619048f, -0.567460f, -0.515873f, -0.464286f,
            -0.412698f, -0.361111f, -0.309524f, -0.257937f, -0.206349f, -0.154762f, -0.103175f, -0.051587f,
             0.000000f,  0.051587f,  0.103175f,  0.154762f,  0.206349f,  0.257937f,  0.309524f,  0.361111f,
             0.412698f,  0.464286f,  0.515873f,  0.567460f,  0.619048f,  0.670635f,  0.698611f,  0.724405f,
             0.750198f,  0.775992f,  0.801786f,  0.827579f,  0.853373f,  0.879167f,  0.904960f,  0.930754f,
             0.951637f,  0.958085f,  0.964534f,  0.970982f,  0.977431f,  0.983879f,  0.990327f,  0.996776f,
        ];

        private readonly byte[] _smallTable = [
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0D, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x11,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0E, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x15,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0D, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x12,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0E, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x19,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0D, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x11,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0E, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x16,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0D, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x12,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0E, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x00,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0D, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x11,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0E, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x15,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0D, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x12,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0E, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x1A,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0D, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x11,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0E, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x16,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0D, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x12,
            0x04, 0x06, 0x05, 0x09, 0x04, 0x06, 0x05, 0x0E, 0x04, 0x06, 0x05, 0x0A, 0x04, 0x06, 0x05, 0x02,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x13, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x17,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x14, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x1B,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x13, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x18,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x14, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x01,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x13, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x17,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x14, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x1C,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x13, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x18,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x14, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x03,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x13, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x17,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x14, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x1B,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x13, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x18,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x14, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x01,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x13, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x17,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x14, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x1C,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x13, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x18,
            0x04, 0x0B, 0x07, 0x0F, 0x04, 0x0C, 0x08, 0x14, 0x04, 0x0B, 0x07, 0x10, 0x04, 0x0C, 0x08, 0x03,
        ];

        private readonly int[] _wordTable = [
                     1,           8,           0,           1,           7,           0,          0,           8,          0,           0,           7,           0,          0,           2,          0,           0,
                     2, -1082130432,           0,           2,  1065353216,           0,          3, -1082130432,          0,           3,  1065353216,           1,          4, -1073741824,          1,           4,
            1073741824,           1,           3, -1073741824,           1,           3, 1073741824,           1,          5, -1069547520,           1,           5, 1077936128,           1,          4, -1069547520,
                     1,           4,  1077936128,           1,           6, -1065353216,          1,           6, 1082130432,           1,           5, -1065353216,          1,           5, 1082130432,           1,
                     7, -1063256064,           1,           7,  1084227584,           1,          6, -1063256064,          1,           6,  1084227584,           1,          8, -1061158912,          1,           8,
            1086324736,           1,           7, -1061158912,           1,           7, 1086324736,   885592027, 1243806083,  1050253722,          20,  1065353216, 1065353216,  1036831949, 1061997773,  1117782016,
            1117782016,  1031127695,  1050253722,  1045220557,  1058642330,  1061997773, 1065353216,    23896536,          0,     4194304,           0,   149718852,  149718852,           0,   27225808,    27225816,
                     1,           0, -1082130432,  1077936128, -1069547520,  1065353216, 1077936128, -1061158912, 1077936128,           0, -1069547520,  1077936128,          0,           0, 1065353216,           0,
                     0,           0,    24903876,           0, 
        ];

        private int _offset     = 0;
        private int _activeByte = 0;
        private int _comparison = 8;

        private byte[] _chunkBuffer;

        private readonly float[] _firstLookup = new float[12];
        private readonly float[] _lookup      = new float[12];
        private readonly float[] _decodeTable = new float[1000];
        private readonly float[] _collection  = new float[120];
        private readonly float[] _data        = new float[348];
        private readonly float[] _espResult   = new float[12];
        private readonly float[] _otherResult = new float[12];

        public byte[] WaveBuffer { get; private set; }

        public CBX(string filePath)
        {
            Convert(File.ReadAllBytes(filePath));
        }

        public CBX(byte[] buffer)
        {
            Convert(buffer);
        }

        private void Convert(byte[] buffer)
        {
            // Initialize.

            for (int i = 0; i < _exponentTable.Length; ++i)
            {
                _exponentTable[i] = (float)(59.9246f * Math.Pow(1.068f, i + 1));
            }

            // Read Header.

            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            if (reader.ReadUInt32AsString() != MagicBox)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            int outputSize = reader.ReadInt32();
            int sampleRate = reader.ReadInt32();

            // Convert the file to WAV.

            _chunkBuffer = reader.ReadBytes((int)(stream.Length - stream.Position));
            _activeByte  = _offset + 1 > _chunkBuffer.Length ? (byte)0 : _chunkBuffer[_offset++];

            int      chunkOffset = 0;
            ushort[] chunkData   = new ushort[outputSize];

            while (_offset < _chunkBuffer.Length)
            {
                GenerateDecodeTable();

                for (int i = 0; i < ChunkSize; i++)
                {
                    uint value = BitConverter.SingleToUInt32Bits(_decodeTable[i]) & 0x1FFFF;
                    if (value - 0x8000 < 0x10000)
                    {
                        value = (uint)(0x8000 - (value < 0x10000 ? 1 : 0));
                    }

                    chunkData[chunkOffset + i] = (ushort)value;

                    _decodeTable[i] = 0.0f;
                }

                chunkOffset += ChunkSize;
            }

            // Write WAV header.

            using MemoryStream outputStream = new();
            using BinaryWriter writer = new(outputStream);

            writer.WriteStringWithoutPrefixedSize("RIFF");
            writer.Write(2 * chunkOffset + 32);
            writer.WriteStringWithoutPrefixedSize("WAVE");
            writer.WriteStringWithoutPrefixedSize("fmt ");
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(sampleRate);
            writer.Write(sampleRate * 2);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.WriteStringWithoutPrefixedSize("data");
            writer.Write(2 * chunkOffset);

            // Write WAV data.

            for (int i = 0; i < chunkOffset; i++)
            {
                writer.Write(chunkData[i]);
            }

            WaveBuffer = outputStream.ToArray();
        }

        private void RefreshActiveByte(int value)
        {
            _comparison -= value;
            _activeByte >>= value;

            if (_comparison >= 8)
            {
                return;
            }

            _activeByte |= (_offset + 1 > _chunkBuffer.Length ? (byte)0 : _chunkBuffer[_offset++]) << _comparison;
            _comparison += 8;
        }

        private void GenerateDecodeTable()
        {
            bool unknownFlag = (_activeByte & 63) < 24;

            for (int i = 0; i < 4; i++)
            {
                _lookup[i] = (float)((_floatTable[_activeByte & 63] - _data[i]) * 0.25);

                RefreshActiveByte(6);
            }

            for (int i = 4; i < 12; i++)
            {
                _lookup[i] = (float)((_floatTable[(_activeByte & 31) + 16] - _data[i]) * 0.25);

                RefreshActiveByte(5);
            }

            int unk0 = 0;

            for (int i = 216; i < 648; i += 108)
            {
                int unk1 = _activeByte & byte.MaxValue;

                RefreshActiveByte(8);

                int offset = i - unk1;
                float unk2 = (_activeByte & 15) * 0.06666667f;

                RefreshActiveByte(4);

                float unk3 = _exponentTable[_activeByte & 63];

                RefreshActiveByte(6);

                int unk4 = _activeByte & 1;

                RefreshActiveByte(1);

                int unk5 = _activeByte & 1;

                RefreshActiveByte(1);

                int collectionOffsetBase = 8;
                
                if (!unknownFlag)
                {
                    int unk6 = 0;

                    do
                    {
                        switch (_activeByte & 3)
                        {
                            case 0:
                            case 2:
                                {
                                    _collection[collectionOffsetBase - 1 + unk4 + unk6] = 0.0f;

                                    RefreshActiveByte(1);
                                    break;
                                }
                            case 1:
                                {
                                    _collection[collectionOffsetBase - 1 + unk4 + unk6] = -2f;

                                    RefreshActiveByte(2);
                                    break;
                                }
                            case 3:
                                {
                                    _collection[collectionOffsetBase - 1 + unk4 + unk6] = 2f;

                                    RefreshActiveByte(2);
                                    break;
                                }
                        }

                        unk6 += 2;
                    }
                    while (unk6 < 108);
                }
                else
                {
                    int unk7 = 0;
                    int unk8 = 0;

                    do
                    {
                        byte unk9 = _smallTable[(_activeByte & byte.MaxValue) + (unk7 << 8)];
                        unk7 = _wordTable[unk9 * 3];

                        RefreshActiveByte(_wordTable[(unk9 * 3) + 1]);

                        if (unk9 < 4)
                        {
                            if (unk9 < 2)
                            {
                                int unk10 = 7;

                                while (true)
                                {
                                    int unk11 = _activeByte & 1;

                                    RefreshActiveByte(1);

                                    if (unk11 == 1)
                                    {
                                        unk10++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                int unk12 = _activeByte & 1;

                                RefreshActiveByte(1);

                                _collection[collectionOffsetBase - 1 + unk4 + unk8] = unk12 != 1 ? -unk10 : unk10;

                                unk8 += 2;
                            }
                            else
                            {
                                int unk13 = (_activeByte & 63) + 7;

                                RefreshActiveByte(6);

                                if (unk13 * 2 + unk8 > 108)
                                {
                                    unk13 = (108 - unk8) / 2;
                                }

                                if (unk13 > 0)
                                {
                                    do
                                    {
                                        _collection[collectionOffsetBase - 1 + unk4 + unk8] = 0.0f;
                                        unk8 += 2;
                                        unk13--;
                                    }
                                    while (unk13 != 0);
                                }
                            }
                        }
                        else
                        {
                            _collection[collectionOffsetBase - 1 + unk4 + unk8] = BitConverter.Int32BitsToSingle(_wordTable[(unk9 * 3) + 2]);
                            unk8 += 2;
                        }
                    }
                    while (unk8 <= 107);
                }

                if (unk5 == 0)
                {
                    for (int j = collectionOffsetBase + 1 - unk4; j < 107 + collectionOffsetBase + 1 - unk4; j += 2)
                    {
                        _collection[j - 1] = (_collection[j - 2] + _collection[j]) * 0.597385942935944f - (_collection[j - 4] + _collection[j + 2]) * 0.114591561257839f + (_collection[j - 6] + _collection[j + 4]) * 0.0180326793342829f;
                    }

                    unk3 *= 0.5f;
                }
                else
                {
                    for (int j = 0; j < 54; ++j)
                    {
                        _collection[collectionOffsetBase - unk4 + j * 2] = 0.0f;
                    }
                }

                int unk14 = offset + 1;
                int collectionOffset = collectionOffsetBase;

                for (int j = 0; j < 12; ++j)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        _decodeTable[unk0 + k] = _collection[collectionOffset + (k - 1)] * unk3 + (unk14 + (k - 1) >= 324 ? _decodeTable[unk14 + 27 - (352 - k)] : _data[unk14 + 27 + (k - 4)]) * unk2;
                    }

                    collectionOffset += 9;
                    unk14 += 9;
                    unk0 += 9;
                }
            }

            for (int i = 0; i < 324; i++)
            {
                _data[24 + i] = _decodeTable[108 + i];
            }

            int unk15 = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j != 12; j += 6)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        _data[j + k] += _lookup[j + k];
                    }
                }

                int unk16 = i != 3 ? 1 : 33;

                for (int j = 11; j > 0; j--)
                {
                    _firstLookup[j] = _data[j - 1];
                }

                _firstLookup[0] = 1.0f;

                for (int j = 0; j < 12; j++)
                {
                    float unk17 = -_data[11] * _firstLookup[11] - _data[10] * _firstLookup[10];

                    for (int k = 10; k > 0; k--)
                    {
                        _firstLookup[k + 1] = _firstLookup[k] + _data[k] * unk17;
                        unk17 -= _data[k - 1] * _firstLookup[k - 1];
                    }

                    _firstLookup[1] = _firstLookup[0] + _data[0] * unk17;
                    _firstLookup[0] = unk17;
                    _espResult[j] = unk17;

                    int unk18 = 0;
                    if (j > 3)
                    {
                        int unk19 = (j - 4 >> 2) + 1;
                        unk18 = unk19 * 4;
                        int unk20 = 0;
                        do
                        {
                            unk19--;
                            unk17 = unk17 - _espResult[j - 1 - unk20 * 4] * _otherResult[unk20 * 4] - _espResult[j - 2 - unk20 * 4] * _otherResult[unk20 * 4 + 1] - _espResult[j - 3 - unk20 * 4] * _otherResult[unk20 * 4 + 2] - _espResult[j - 4 - unk20 * 4] * _otherResult[unk20 * 4 + 3];
                            unk20++;
                        }
                        while (unk19 != 0);
                    }

                    if (j > unk18)
                    {
                        do
                        {
                            unk17 -= _espResult[j - unk18 - 1] * _otherResult[unk18];
                            ++unk18;
                        }
                        while (j > unk18);
                    }

                    _otherResult[j] = unk17;
                }

                if (unk16 <= 0)
                {
                    return;
                }

                do
                {
                    List<int> listIndex = [23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12];

                    int checkIndex(int i)
                    {
                        if (i == listIndex.Count)
                        {
                            i = 0;
                        }

                        return i % listIndex.Count;
                    }

                    for (int k = 0; k < 12; k++)
                    {

                        _data[listIndex[checkIndex(k + 12)]] = (_data[listIndex[checkIndex(k + 11)]] * _otherResult[0] + _decodeTable[unk15 + k] + _data[listIndex[checkIndex(k + 10)]] * _otherResult[1] + _data[listIndex[checkIndex(k + 9)]] * _otherResult[2] + _data[listIndex[checkIndex(k + 8)]] * _otherResult[3] + _data[listIndex[checkIndex(k + 7)]] * _otherResult[4] + _data[listIndex[checkIndex(k + 6)]] * _otherResult[5] + _data[listIndex[checkIndex(k + 5)]] * _otherResult[6] + _data[listIndex[checkIndex(k + 4)]] * _otherResult[7] + _data[listIndex[checkIndex(k + 3)]] * _otherResult[8] + _data[listIndex[checkIndex(k + 2)]] * _otherResult[9] + _data[listIndex[checkIndex(k + 1)]] * _otherResult[10] + _data[listIndex[checkIndex(k)]] * _otherResult[11]);
                        _decodeTable[unk15 + k]              = _data[listIndex[checkIndex(k + 12)]] + 12582912f;
                    }
 
                    unk15 += 12;
                    --unk16;
                }
                while (unk16 != 0);
            }
        }
    }
}