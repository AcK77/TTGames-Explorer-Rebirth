using System.IO.Compression;

namespace TTGamesExplorerRebirthLib.Compression
{
    public static class Inflate
    {
        /// <summary>
        ///     Give source byte buffer and return destination byte buffer, process to Inflate decompression.
        ///     This algorithm is mostly used by TTFusion.
        /// </summary>
        /// <param name="buffer">
        ///     Byte array representing the source data to be decompressed.
        /// </param>
        /// <returns>
        ///     Byte array representing the decompressed destination data.
        /// </returns>
        public static byte[] Decompress(byte[] buffer)
        {
            using MemoryStream  decompressedStream = new();
            using MemoryStream  compressStream     = new(buffer);
            using DeflateStream deflateStream      = new(compressStream, CompressionMode.Decompress);

            deflateStream.CopyTo(decompressedStream);

            return decompressedStream.ToArray();
        }
    }
}
