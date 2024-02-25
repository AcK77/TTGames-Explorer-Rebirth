using TTGamesExplorerRebirthLib.Compression;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    /// <summary>
    ///     Give LZ2K file data and decompress it by applying UnLZ2K algo.
    /// </summary>
    /// <remarks>
    ///     Based on QuickBMS script by Luigi Auriemma:
    ///     https://aluigi.altervista.org/quickbms.htm
    ///     
    ///     Based on my own research (Ac_K).
    /// </remarks>
    public static class LZ2K
    {
        private const string Magic = "LZ2K";

        public static byte[] Decompress(byte[] fileBuffer)
        {
            using MemoryStream inputStream  = new(fileBuffer);
            using MemoryStream outputStream = new();
            using BinaryReader reader       = new(inputStream);
            using BinaryWriter writer       = new(outputStream);

            uint bytesLeft = (uint)inputStream.Length;
            while (bytesLeft != 0)
            {
                if (reader.ReadUInt32AsString() != Magic)
                {
                    throw new InvalidDataException($"{inputStream.Position:x8}");
                }

                uint decompressedSize = reader.ReadUInt32();
                uint compressedSize   = reader.ReadUInt32();

                byte[] inputBuffer = new byte[compressedSize];

                Array.Copy(fileBuffer, inputStream.Position, inputBuffer, 0, compressedSize);

                inputStream.Seek(compressedSize, SeekOrigin.Current);

                byte[] outputBuffer = new byte[decompressedSize];

                if (compressedSize != decompressedSize)
                {
                    UnLZ2K.Decompress(inputBuffer, outputBuffer);
                }
                else
                {
                    Array.Copy(inputBuffer, outputBuffer, decompressedSize);
                }

                writer.Write(outputBuffer);

                bytesLeft -= compressedSize + 0xC;
            }

            return outputStream.ToArray();
        }
    }
}