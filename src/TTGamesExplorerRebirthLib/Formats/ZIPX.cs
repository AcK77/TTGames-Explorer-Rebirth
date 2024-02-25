using TTGamesExplorerRebirthLib.Encryption;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    /// <summary>
    ///     Give ZIPX file data and decrypt it by applying RC4 cryptography.
    /// </summary>
    /// <remarks>
    ///     Based on QuickBMS script by Luigi Auriemma:
    ///     https://aluigi.altervista.org/quickbms.htm
    /// </remarks>
    public static class ZIPX
    {
        private const string Magic = "ZIPX";

        public static byte[] Decrypt(byte[] fileBuffer)
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

                uint compressedSize   = reader.ReadUInt32();
                uint decompressedSize = reader.ReadUInt32();

                byte[] inputBuffer = new byte[decompressedSize];

                Array.Copy(fileBuffer, inputStream.Position, inputBuffer, 0, decompressedSize);

                inputStream.Seek(decompressedSize, SeekOrigin.Current);

                byte[] outputBuffer = RC4.Crypt(inputBuffer, BitConverter.GetBytes(decompressedSize));

                writer.Write(outputBuffer);

                bytesLeft -= decompressedSize + 0xC;
            }

            return outputStream.ToArray();
        }
    }
}