using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    /// <summary>
    ///     Give Deflate_v1.0 file data and decompress it by applying Deflate.
    ///     It uses RFC 1951.
    /// </summary>
    /// <remarks>
    ///     Based on my own research (Ac_K).
    ///     
    ///     FIXME: AN4 files won't decompress.
    /// </remarks>
    public static class Deflatev1
    {
        private const string Magic = "Deflate_v1.0";

        public static byte[] Decompress(byte[] fileBuffer)
        {
            using MemoryStream stream = new(fileBuffer);
            using BinaryReader reader = new(stream);

            if (reader.ReadNullTerminatedString() != Magic)
            {
                stream.Seek(0x20, SeekOrigin.Begin);

                uint decompressedSize = reader.ReadUInt32();
                uint compressedSize   = (uint)(stream.Length - stream.Position);

                byte[] buffer = new byte[compressedSize];

                Array.Copy(fileBuffer, 0x24, buffer, 0, compressedSize);

                MemoryStream outputStream = new();

                using MemoryStream        compressedStream = new(buffer);
                using InflaterInputStream inputStream      = new(compressedStream, new Inflater(true));

                inputStream.CopyTo(outputStream);
            }

            throw new InvalidDataException($"{stream.Position:x8}");
        }
    }
}
