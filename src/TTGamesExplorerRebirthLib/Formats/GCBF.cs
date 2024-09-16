using K4os.Compression.LZ4;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    /// <summary>
    ///     Give GCBF file data and decompress it by applying LZ4 algo.
    /// </summary>
    /// <remarks>
    ///     Based on QuickBMS script by Luigi Auriemma:
    ///     https://aluigi.altervista.org/quickbms.htm
    ///     
    ///     Based on my own research (Ac_K).
    /// </remarks>
    public static class GCBF
    {
        public const string MagicGCBF = "GCBF";
        public const string MagicPadA = "padA";
        public const string MagicLz4  = " 4zL";

        public static byte[] Decompress(byte[] fileBuffer)
        {
            using MemoryStream inputStream  = new(fileBuffer);
            using MemoryStream outputStream = new();
            using BinaryReader reader       = new(inputStream);
            using BinaryWriter writer       = new(outputStream);

            if (reader.ReadUInt32AsString() != MagicGCBF)
            {
                throw new InvalidDataException($"{inputStream.Position:x8}");
            }

            uint chunksCount = reader.ReadUInt32() - 1;
            uint padding1    = reader.ReadUInt32();
            uint fileOffset  = reader.ReadUInt32();

            for (int i = 0; i < chunksCount; i++)
            {
                // NOTE: We don't care about those values since they are in the chunk header too.
                uint decompressedSize = reader.ReadUInt32();
                uint endOfFileOffset  = reader.ReadUInt32();
            }

            for (int i = 0; i < chunksCount; i++)
            {
                if (reader.ReadUInt32AsString() != MagicPadA)
                {
                    throw new InvalidDataException($"{inputStream.Position:x8}");
                }

                if (reader.ReadUInt32AsString() != MagicLz4)
                {
                    throw new InvalidDataException($"{inputStream.Position:x8}");
                }

                uint  compressedSize          = reader.ReadUInt32();
                uint  decompressedSize        = reader.ReadUInt32();
                uint  unknownCompressedHash   = reader.ReadUInt32();
                uint  unknownDecompressedHash = reader.ReadUInt32();
                uint  decompressedSize2       = reader.ReadUInt32();
                ulong padding2                = reader.ReadUInt64();

                byte[] chunkBuffer             = reader.ReadBytes((int)compressedSize);
                byte[] decompressedChunkBuffer = new byte[decompressedSize];

                LZ4Codec.Decode(chunkBuffer, 0, chunkBuffer.Length, decompressedChunkBuffer, 0, decompressedChunkBuffer.Length);

                writer.Write(decompressedChunkBuffer);
            }

            return outputStream.ToArray();
        }
    }
}