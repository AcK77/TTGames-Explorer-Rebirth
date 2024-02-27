using SixLabors.ImageSharp;
using System.Runtime.InteropServices;
using System.Text;
using TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Decoder;
using TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared;
using TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared.ImageFiles;
using TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.ImageSharp;

namespace TTGamesExplorerRebirthLib.Formats.DDS
{
    public class DDSImage
    {
        public Image[] Images;
        public string Type;
        public byte[] Data;

        public DDSImage(byte[] buffer)
        {
            Data = buffer;

            using MemoryStream stream = new(buffer);

            DdsFile ddsFile = DdsFile.Load(stream);
            BcDecoder bcDecoder = new();

            Type = Encoding.ASCII.GetString(BitConverter.GetBytes(ddsFile.header.ddsPixelFormat.dwFourCc), 0, 4);

            if (ddsFile.header.dwMipMapCount != 0)
            {
                Images = bcDecoder.DecodeAllMipMapsToImageRgba32(ddsFile);
            }
            else
            {
                Images = new Image[1];
                Images[0] = bcDecoder.DecodeToImageRgba32(ddsFile);
            }
        }
        ~DDSImage() 
        {
            Data = null;
            Images = null;
            GC.Collect();
        }
        public static uint CalculateDdsSize(MemoryStream stream, BinaryReader reader)
        {
            _ = reader.ReadUInt32(); // Magic: DDS.

            DdsHeader ddsHeader = MemoryMarshal.Cast<byte, DdsHeader>(reader.ReadBytes(Marshal.SizeOf<DdsHeader>()).AsSpan())[0];
            uint ddsSize = (uint)Marshal.SizeOf<DdsHeader>() + 4;

            DdsHeaderDx10 dx10Header = new();

            if (ddsHeader.ddsPixelFormat.IsDxt10Format)
            {
                ddsSize += (uint)Marshal.SizeOf<DdsHeaderDx10>();
                dx10Header = MemoryMarshal.Cast<byte, DdsHeaderDx10>(reader.ReadBytes(Marshal.SizeOf<DdsHeaderDx10>()).AsSpan())[0];

                stream.Seek(-Marshal.SizeOf<DdsHeaderDx10>(), SeekOrigin.Current);
            }

            stream.Seek(-(Marshal.SizeOf<DdsHeader>() + 4), SeekOrigin.Current);

            uint mipMapCount = Math.Max(1, ddsHeader.dwMipMapCount);
            uint faceCount = (ddsHeader.dwCaps2 & HeaderCaps2.Ddscaps2Cubemap) != 0 ? 6u : 1u;

            for (var face = 0; face < faceCount; face++)
            {
                DxgiFormat format = ddsHeader.ddsPixelFormat.IsDxt10Format ? dx10Header.dxgiFormat : ddsHeader.ddsPixelFormat.DxgiFormat;
                uint sizeInBytes = DdsFile.GetSizeInBytes(format, ddsHeader.dwWidth, ddsHeader.dwHeight);

                for (int mip = 0; mip < mipMapCount; mip++)
                {
                    MipMapper.CalculateMipLevelSize((int)ddsHeader.dwWidth, (int)ddsHeader.dwHeight, mip, out var mipWidth, out var mipHeight);

                    if (mip > 0) // Calculate new sizeInBytes.
                    {
                        sizeInBytes = DdsFile.GetSizeInBytes(format, (uint)mipWidth, (uint)mipHeight);
                    }

                    ddsSize += sizeInBytes;
                }
            }

            return ddsSize;
        }
    }
}