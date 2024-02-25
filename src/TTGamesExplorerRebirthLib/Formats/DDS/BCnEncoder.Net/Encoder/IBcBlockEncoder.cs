using TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared;
using TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared.ImageFiles;

namespace TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Encoder
{
    internal interface IBcBlockEncoder<T> where T : unmanaged
    {
        byte[] Encode(T[] blocks, int blockWidth, int blockHeight, CompressionQuality quality, OperationContext context);
        void EncodeBlock(T block, CompressionQuality quality, Span<byte> output);
        GlInternalFormat GetInternalFormat();
        GlFormat GetBaseInternalFormat();
        DxgiFormat GetDxgiFormat();
        int GetBlockSize();
    }
}
