using System.Runtime.InteropServices;
using System.Text;
using TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared;

namespace TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared.ImageFiles
{
    public class DdsFile
    {
        public DdsHeader header;
        public DdsHeaderDx10 dx10Header;
        public List<DdsFace> Faces { get; } = [];

        public DdsFile() { }
        public DdsFile(DdsHeader header)
        {
            this.header = header;
        }

        public DdsFile(DdsHeader header, DdsHeaderDx10 dx10Header)
        {
            this.header = header;
            this.dx10Header = dx10Header;
        }

        public static DdsFile Load(Stream s)
        {
            using var br = new BinaryReader(s, Encoding.UTF8, true);
            var magic = br.ReadUInt32();
            if (magic != 0x20534444U)
            {
                throw new FormatException("The file does not contain a dds file.");
            }
            var header = br.ReadStruct<DdsHeader>();
            DdsHeaderDx10 dx10Header = default;
            if (header.dwSize != 124)
            {
                throw new FormatException("The file header contains invalid dwSize.");
            }

            var dx10Format = header.ddsPixelFormat.IsDxt10Format;

            DdsFile output;

            if (dx10Format)
            {
                dx10Header = br.ReadStruct<DdsHeaderDx10>();
                output = new DdsFile(header, dx10Header);
            }
            else
            {
                output = new DdsFile(header);
            }

            var mipMapCount = Math.Max(1, header.dwMipMapCount);
            var faceCount = (header.dwCaps2 & HeaderCaps2.Ddscaps2Cubemap) != 0 ? 6u : 1u;
            var width = header.dwWidth;
            var height = header.dwHeight;

            for (var face = 0; face < faceCount; face++)
            {
                var format = dx10Format ? dx10Header.dxgiFormat : header.ddsPixelFormat.DxgiFormat;
                var sizeInBytes = GetSizeInBytes(format, width, height);

                output.Faces.Add(new DdsFace(width, height, sizeInBytes, (int)mipMapCount));

                for (var mip = 0; mip < mipMapCount; mip++)
                {
                    MipMapper.CalculateMipLevelSize(
                        (int)header.dwWidth,
                        (int)header.dwHeight,
                        mip,
                        out var mipWidth,
                        out var mipHeight);

                    if (mip > 0) //Calculate new byteSize
                    {
                        sizeInBytes = GetSizeInBytes(format, (uint)mipWidth, (uint)mipHeight);
                    }

                    var data = new byte[sizeInBytes];
                    br.Read(data);
                    output.Faces[face].MipMaps[mip] = new DdsMipMap(data, (uint)mipWidth, (uint)mipHeight);
                }
            }

            return output;
        }

        public void Write(Stream outputStream)
        {
            if (Faces.Count < 1 || Faces[0].MipMaps.Length < 1)
            {
                throw new InvalidOperationException("The DDS structure should have at least 1 mipmap level and 1 Face before writing to file.");
            }

            header.dwFlags |= HeaderFlags.Required;

            header.dwMipMapCount = (uint)Faces[0].MipMaps.Length;
            if (header.dwMipMapCount > 1) // MipMaps
            {
                header.dwCaps |= HeaderCaps.DdscapsMipmap | HeaderCaps.DdscapsComplex;
            }
            if (Faces.Count == 6) // CubeMap
            {
                header.dwCaps |= HeaderCaps.DdscapsComplex;
                header.dwCaps2 |= HeaderCaps2.Ddscaps2Cubemap |
                                  HeaderCaps2.Ddscaps2CubemapPositivex |
                                  HeaderCaps2.Ddscaps2CubemapNegativex |
                                  HeaderCaps2.Ddscaps2CubemapPositivey |
                                  HeaderCaps2.Ddscaps2CubemapNegativey |
                                  HeaderCaps2.Ddscaps2CubemapPositivez |
                                  HeaderCaps2.Ddscaps2CubemapNegativez;
            }

            header.dwWidth = Faces[0].Width;
            header.dwHeight = Faces[0].Height;

            for (var i = 0; i < Faces.Count; i++)
            {
                if (Faces[i].Width != header.dwWidth || Faces[i].Height != header.dwHeight)
                {
                    throw new InvalidOperationException("Faces with different sizes are not supported.");
                }
            }

            var faceCount = Faces.Count;
            var mipCount = (int)header.dwMipMapCount;

            using var bw = new BinaryWriter(outputStream, Encoding.UTF8, true);
            bw.Write(0x20534444U); // magic 'DDS '

            bw.WriteStruct(header);

            if (header.ddsPixelFormat.IsDxt10Format)
            {
                bw.WriteStruct(dx10Header);
            }

            for (var face = 0; face < faceCount; face++)
            {
                for (var mip = 0; mip < mipCount; mip++)
                {
                    bw.Write(Faces[face].MipMaps[mip].Data);
                }
            }
        }

        public static uint GetSizeInBytes(DxgiFormat format, uint width, uint height)
        {
            uint sizeInBytes;
            if (format.IsCompressedFormat())
            {
                sizeInBytes = (uint)ImageToBlocks.CalculateNumOfBlocks((int)width, (int)height);
                sizeInBytes *= (uint)format.GetByteSize();
            }
            else
            {
                sizeInBytes = width * height;
                sizeInBytes = (uint)(sizeInBytes * format.GetByteSize());
            }

            return sizeInBytes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DdsHeader
    {
        /// <summary>
        /// Has to be 124
        /// </summary>
        public uint dwSize;
        public HeaderFlags dwFlags;
        public uint dwHeight;
        public uint dwWidth;
        public uint dwPitchOrLinearSize;
        public uint dwDepth;
        public uint dwMipMapCount;
        public fixed uint dwReserved1[11];
        public DdsPixelFormat ddsPixelFormat;
        public HeaderCaps dwCaps;
        public HeaderCaps2 dwCaps2;
        public uint dwCaps3;
        public uint dwCaps4;
        public uint dwReserved2;

        public static (DdsHeader, DdsHeaderDx10) InitializeCompressed(int width, int height, DxgiFormat format, bool preferDxt10Header)
        {
            var header = new DdsHeader();
            var dxt10Header = new DdsHeaderDx10();

            header.dwSize = 124;
            header.dwFlags = HeaderFlags.Required;
            header.dwWidth = (uint)width;
            header.dwHeight = (uint)height;
            header.dwDepth = 1;
            header.dwMipMapCount = 1;
            header.dwCaps = HeaderCaps.DdscapsTexture;

            if (preferDxt10Header)
            {
                // ATC formats cannot be written to DXT10 header due to lack of a DxgiFormat enum
                switch (format)
                {
                    case DxgiFormat.DxgiFormatAtcExt:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Atc
                        };
                        break;

                    case DxgiFormat.DxgiFormatAtcExplicitAlphaExt:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Atci
                        };
                        break;

                    case DxgiFormat.DxgiFormatAtcInterpolatedAlphaExt:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Atca
                        };
                        break;

                    default:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Dx10
                        };
                        dxt10Header.arraySize = 1;
                        dxt10Header.dxgiFormat = format;
                        dxt10Header.resourceDimension = D3D10ResourceDimension.D3D10ResourceDimensionTexture2D;
                        break;
                }
            }
            else
            {
                switch (format)
                {
                    case DxgiFormat.DxgiFormatBc1Unorm:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Dxt1
                        };
                        break;

                    case DxgiFormat.DxgiFormatBc2Unorm:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Dxt3
                        };
                        break;

                    case DxgiFormat.DxgiFormatBc3Unorm:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Dxt5
                        };
                        break;

                    case DxgiFormat.DxgiFormatBc4Unorm:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Bc4U
                        };
                        break;

                    case DxgiFormat.DxgiFormatBc5Unorm:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Ati2
                        };
                        break;

                    case DxgiFormat.DxgiFormatAtcExt:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Atc
                        };
                        break;

                    case DxgiFormat.DxgiFormatAtcExplicitAlphaExt:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Atci
                        };
                        break;

                    case DxgiFormat.DxgiFormatAtcInterpolatedAlphaExt:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Atca
                        };
                        break;

                    default:
                        header.ddsPixelFormat = new DdsPixelFormat
                        {
                            dwSize = 32,
                            dwFlags = PixelFormatFlags.DdpfFourcc,
                            dwFourCc = DdsPixelFormat.Dx10
                        };
                        dxt10Header.arraySize = 1;
                        dxt10Header.dxgiFormat = format;
                        dxt10Header.resourceDimension = D3D10ResourceDimension.D3D10ResourceDimensionTexture2D;
                        break;
                }
            }

            return (header, dxt10Header);
        }

        public static DdsHeader InitializeUncompressed(int width, int height, DxgiFormat format)
        {
            var header = new DdsHeader
            {
                dwSize = 124,
                dwFlags = HeaderFlags.Required | HeaderFlags.DdsdPitch,
                dwWidth = (uint)width,
                dwHeight = (uint)height,
                dwDepth = 1,
                dwMipMapCount = 1,
                dwCaps = HeaderCaps.DdscapsTexture
            };

            if (format == DxgiFormat.DxgiFormatR8Unorm)
            {
                header.ddsPixelFormat = new DdsPixelFormat()
                {
                    dwSize = 32,
                    dwFlags = PixelFormatFlags.DdpfLuminance,
                    dwRgbBitCount = 8,
                    dwRBitMask = 0xFF
                };
                header.dwPitchOrLinearSize = (uint)((width * 8 + 7) / 8);
            }
            else if (format == DxgiFormat.DxgiFormatR8G8Unorm)
            {
                header.ddsPixelFormat = new DdsPixelFormat()
                {
                    dwSize = 32,
                    dwFlags = PixelFormatFlags.DdpfLuminance | PixelFormatFlags.DdpfAlphaPixels,
                    dwRgbBitCount = 16,
                    dwRBitMask = 0xFF,
                    dwGBitMask = 0xFF00
                };
                header.dwPitchOrLinearSize = (uint)((width * 16 + 7) / 8);
            }
            else if (format == DxgiFormat.DxgiFormatR8G8B8A8Unorm)
            {
                header.ddsPixelFormat = new DdsPixelFormat()
                {
                    dwSize = 32,
                    dwFlags = PixelFormatFlags.DdpfRgb | PixelFormatFlags.DdpfAlphaPixels,
                    dwRgbBitCount = 32,
                    dwRBitMask = 0xFF,
                    dwGBitMask = 0xFF00,
                    dwBBitMask = 0xFF0000,
                    dwABitMask = 0xFF000000,
                };
                header.dwPitchOrLinearSize = (uint)((width * 32 + 7) / 8);
            }
            else if (format == DxgiFormat.DxgiFormatB8G8R8A8Unorm)
            {
                header.ddsPixelFormat = new DdsPixelFormat()
                {
                    dwSize = 32,
                    dwFlags = PixelFormatFlags.DdpfRgb | PixelFormatFlags.DdpfAlphaPixels,
                    dwRgbBitCount = 32,
                    dwRBitMask = 0xFF0000,
                    dwGBitMask = 0xFF00,
                    dwBBitMask = 0xFF,
                    dwABitMask = 0xFF000000,
                };
                header.dwPitchOrLinearSize = (uint)((width * 32 + 7) / 8);
            }
            else
            {
                throw new NotImplementedException("This Format is not implemented in this method");
            }

            return header;
        }
    }

    public struct DdsPixelFormat
    {
        public static readonly uint Dx10 = MakeFourCc('D', 'X', '1', '0');

        public static readonly uint Dxt1 = MakeFourCc('D', 'X', 'T', '1');
        public static readonly uint Dxt2 = MakeFourCc('D', 'X', 'T', '2');
        public static readonly uint Dxt3 = MakeFourCc('D', 'X', 'T', '3');
        public static readonly uint Dxt4 = MakeFourCc('D', 'X', 'T', '4');
        public static readonly uint Dxt5 = MakeFourCc('D', 'X', 'T', '5');
        public static readonly uint Ati1 = MakeFourCc('A', 'T', 'I', '1');
        public static readonly uint Ati2 = MakeFourCc('A', 'T', 'I', '2');
        public static readonly uint Atc = MakeFourCc('A', 'T', 'C', ' ');
        public static readonly uint Atci = MakeFourCc('A', 'T', 'C', 'I');
        public static readonly uint Atca = MakeFourCc('A', 'T', 'C', 'A');

        public static readonly uint Bc4S = MakeFourCc('B', 'C', '4', 'S');
        public static readonly uint Bc4U = MakeFourCc('B', 'C', '4', 'U');
        public static readonly uint Bc5S = MakeFourCc('B', 'C', '5', 'S');
        public static readonly uint Bc5U = MakeFourCc('B', 'C', '5', 'U');

        private static uint MakeFourCc(char c0, char c1, char c2, char c3)
        {
            uint result = c0;
            result |= (uint)c1 << 8;
            result |= (uint)c2 << 16;
            result |= (uint)c3 << 24;
            return result;
        }

        public uint dwSize;
        public PixelFormatFlags dwFlags;
        public uint dwFourCc;
        public uint dwRgbBitCount;
        public uint dwRBitMask;
        public uint dwGBitMask;
        public uint dwBBitMask;
        public uint dwABitMask;

        public readonly DxgiFormat DxgiFormat
        {
            get
            {
                if (dwFlags.HasFlag(PixelFormatFlags.DdpfFourcc))
                {
                    if (dwFourCc == Dxt1) return DxgiFormat.DxgiFormatBc1Unorm;
                    if (dwFourCc == Dxt2 || dwFourCc == Dxt3) return DxgiFormat.DxgiFormatBc2Unorm;
                    if (dwFourCc == Dxt4 || dwFourCc == Dxt5) return DxgiFormat.DxgiFormatBc3Unorm;
                    if (dwFourCc == Ati1 || dwFourCc == Bc4S || dwFourCc == Bc4U) return DxgiFormat.DxgiFormatBc4Unorm;
                    if (dwFourCc == Ati2 || dwFourCc == Bc5S || dwFourCc == Bc5U) return DxgiFormat.DxgiFormatBc5Unorm;
                    if (dwFourCc == Atc) return DxgiFormat.DxgiFormatAtcExt;
                    if (dwFourCc == Atci) return DxgiFormat.DxgiFormatAtcExplicitAlphaExt;
                    if (dwFourCc == Atca) return DxgiFormat.DxgiFormatAtcInterpolatedAlphaExt;
                }
                else
                {
                    if (dwFlags.HasFlag(PixelFormatFlags.DdpfRgb)) // RGB/A
                    {
                        if (dwFlags.HasFlag(PixelFormatFlags.DdpfAlphaPixels)) //RGBA
                        {
                            if (dwRgbBitCount == 32)
                            {
                                if (dwRBitMask == 0xff && dwGBitMask == 0xff00 && dwBBitMask == 0xff0000 &&
                                    dwABitMask == 0xff000000)
                                {
                                    return DxgiFormat.DxgiFormatR8G8B8A8Unorm;
                                }

                                if (dwRBitMask == 0xff0000 && dwGBitMask == 0xff00 && dwBBitMask == 0xff &&
                                    dwABitMask == 0xff000000)
                                {
                                    return DxgiFormat.DxgiFormatB8G8R8A8Unorm;
                                }
                            }
                        }
                        else //RGB
                        {
                            if (dwRgbBitCount == 32)
                            {
                                if (dwRBitMask == 0xff0000 && dwGBitMask == 0xff00 && dwBBitMask == 0xff)
                                {
                                    return DxgiFormat.DxgiFormatB8G8R8X8Unorm;
                                }
                            }
                        }
                    }
                    else if (dwFlags.HasFlag(PixelFormatFlags.DdpfLuminance)) // R/RG
                    {
                        if (dwFlags.HasFlag(PixelFormatFlags.DdpfAlphaPixels)) // RG
                        {
                            if (dwRgbBitCount == 16)
                            {
                                if (dwRBitMask == 0xff && dwGBitMask == 0xff00)
                                {
                                    return DxgiFormat.DxgiFormatR8G8Unorm;
                                }
                            }
                        }
                        else // Luminance only
                        {
                            if (dwRgbBitCount == 8)
                            {
                                if (dwRBitMask == 0xff)
                                {
                                    return DxgiFormat.DxgiFormatR8Unorm;
                                }
                            }
                        }
                    }
                }
                return DxgiFormat.DxgiFormatUnknown;
            }
        }

        public readonly bool IsDxt10Format => (dwFlags & PixelFormatFlags.DdpfFourcc) == PixelFormatFlags.DdpfFourcc
                                     && dwFourCc == Dx10;
    }

    public struct DdsHeaderDx10
    {
        public DxgiFormat dxgiFormat;
        public D3D10ResourceDimension resourceDimension;
        public uint miscFlag;
        public uint arraySize;
        public uint miscFlags2;
    }

    public class DdsFace(uint width, uint height, uint sizeInBytes, int numMipMaps)
    {
        public uint Width { get; set; } = width;
        public uint Height { get; set; } = height;
        public uint SizeInBytes { get; } = sizeInBytes;
        public DdsMipMap[] MipMaps { get; } = new DdsMipMap[numMipMaps];
    }

    public class DdsMipMap(byte[] data, uint width, uint height)
    {
        public uint Width { get; set; } = width;
        public uint Height { get; set; } = height;
        public uint SizeInBytes { get; } = (uint)data.Length;
        public byte[] Data { get; } = data;
    }

    /// <summary>
    /// Flags to indicate which members contain valid data.
    /// </summary>
    [Flags]
    public enum HeaderFlags : uint
    {
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DdsdCaps = 0x1,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DdsdHeight = 0x2,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DdsdWidth = 0x4,
        /// <summary>
        /// Required when pitch is provided for an uncompressed texture.
        /// </summary>
        DdsdPitch = 0x8,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DdsdPixelformat = 0x1000,
        /// <summary>
        /// Required in a mipmapped texture.
        /// </summary>
        DdsdMipmapcount = 0x20000,
        /// <summary>
        /// Required when pitch is provided for a compressed texture.
        /// </summary>
        DdsdLinearsize = 0x80000,
        /// <summary>
        /// Required in a depth texture.
        /// </summary>
        DdsdDepth = 0x800000,

        Required = DdsdCaps | DdsdHeight | DdsdWidth | DdsdPixelformat
    }

    /// <summary>
    /// Specifies the complexity of the surfaces stored.
    /// </summary>
    [Flags]
    public enum HeaderCaps : uint
    {
        /// <summary>
        /// Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
        /// </summary>
        DdscapsComplex = 0x8,
        /// <summary>
        /// Optional; should be used for a mipmap.
        /// </summary>
        DdscapsMipmap = 0x400000,
        /// <summary>
        /// Required
        /// </summary>
        DdscapsTexture = 0x1000
    }

    /// <summary>
    /// Additional detail about the surfaces stored.
    /// </summary>
    [Flags]
    public enum HeaderCaps2 : uint
    {
        /// <summary>
        /// Required for a cube map.
        /// </summary>
        Ddscaps2Cubemap = 0x200,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        Ddscaps2CubemapPositivex = 0x400,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        Ddscaps2CubemapNegativex = 0x800,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        Ddscaps2CubemapPositivey = 0x1000,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        Ddscaps2CubemapNegativey = 0x2000,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        Ddscaps2CubemapPositivez = 0x4000,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        Ddscaps2CubemapNegativez = 0x8000,
        /// <summary>
        /// Required for a volume texture.
        /// </summary>
        Ddscaps2Volume = 0x200000
    }

    [Flags]
    public enum PixelFormatFlags : uint
    {
        /// <summary>
        /// Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
        /// </summary>
        DdpfAlphaPixels = 0x1,
        /// <summary>
        /// Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
        /// </summary>
        DdpfAlpha = 0x2,
        /// <summary>
        /// Texture contains compressed RGB data; dwFourCC contains valid data.
        /// </summary>
        DdpfFourcc = 0x4,
        /// <summary>
        /// Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks (dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
        /// </summary>
        DdpfRgb = 0x40,
        /// <summary>
        /// Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)
        /// </summary>
        DdpfYuv = 0x200,
        /// <summary>
        /// Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
        /// </summary>
        DdpfLuminance = 0x20000
    }

    public enum D3D10ResourceDimension : uint
    {
        D3D10ResourceDimensionUnknown,
        D3D10ResourceDimensionBuffer,
        D3D10ResourceDimensionTexture1D,
        D3D10ResourceDimensionTexture2D,
        D3D10ResourceDimensionTexture3D
    };

    public enum DxgiFormat : uint
    {
        DxgiFormatUnknown,
        DxgiFormatR32G32B32A32Typeless,
        DxgiFormatR32G32B32A32Float,
        DxgiFormatR32G32B32A32Uint,
        DxgiFormatR32G32B32A32Sint,
        DxgiFormatR32G32B32Typeless,
        DxgiFormatR32G32B32Float,
        DxgiFormatR32G32B32Uint,
        DxgiFormatR32G32B32Sint,
        DxgiFormatR16G16B16A16Typeless,
        DxgiFormatR16G16B16A16Float,
        DxgiFormatR16G16B16A16Unorm,
        DxgiFormatR16G16B16A16Uint,
        DxgiFormatR16G16B16A16Snorm,
        DxgiFormatR16G16B16A16Sint,
        DxgiFormatR32G32Typeless,
        DxgiFormatR32G32Float,
        DxgiFormatR32G32Uint,
        DxgiFormatR32G32Sint,
        DxgiFormatR32G8X24Typeless,
        DxgiFormatD32FloatS8X24Uint,
        DxgiFormatR32FloatX8X24Typeless,
        DxgiFormatX32TypelessG8X24Uint,
        DxgiFormatR10G10B10A2Typeless,
        DxgiFormatR10G10B10A2Unorm,
        DxgiFormatR10G10B10A2Uint,
        DxgiFormatR11G11B10Float,
        DxgiFormatR8G8B8A8Typeless,
        DxgiFormatR8G8B8A8Unorm,
        DxgiFormatR8G8B8A8UnormSrgb,
        DxgiFormatR8G8B8A8Uint,
        DxgiFormatR8G8B8A8Snorm,
        DxgiFormatR8G8B8A8Sint,
        DxgiFormatR16G16Typeless,
        DxgiFormatR16G16Float,
        DxgiFormatR16G16Unorm,
        DxgiFormatR16G16Uint,
        DxgiFormatR16G16Snorm,
        DxgiFormatR16G16Sint,
        DxgiFormatR32Typeless,
        DxgiFormatD32Float,
        DxgiFormatR32Float,
        DxgiFormatR32Uint,
        DxgiFormatR32Sint,
        DxgiFormatR24G8Typeless,
        DxgiFormatD24UnormS8Uint,
        DxgiFormatR24UnormX8Typeless,
        DxgiFormatX24TypelessG8Uint,
        DxgiFormatR8G8Typeless,
        DxgiFormatR8G8Unorm,
        DxgiFormatR8G8Uint,
        DxgiFormatR8G8Snorm,
        DxgiFormatR8G8Sint,
        DxgiFormatR16Typeless,
        DxgiFormatR16Float,
        DxgiFormatD16Unorm,
        DxgiFormatR16Unorm,
        DxgiFormatR16Uint,
        DxgiFormatR16Snorm,
        DxgiFormatR16Sint,
        DxgiFormatR8Typeless,
        DxgiFormatR8Unorm,
        DxgiFormatR8Uint,
        DxgiFormatR8Snorm,
        DxgiFormatR8Sint,
        DxgiFormatA8Unorm,
        DxgiFormatR1Unorm,
        DxgiFormatR9G9B9E5Sharedexp,
        DxgiFormatR8G8B8G8Unorm,
        DxgiFormatG8R8G8B8Unorm,
        DxgiFormatBc1Typeless,
        DxgiFormatBc1Unorm,
        DxgiFormatBc1UnormSrgb,
        DxgiFormatBc2Typeless,
        DxgiFormatBc2Unorm,
        DxgiFormatBc2UnormSrgb,
        DxgiFormatBc3Typeless,
        DxgiFormatBc3Unorm,
        DxgiFormatBc3UnormSrgb,
        DxgiFormatBc4Typeless,
        DxgiFormatBc4Unorm,
        DxgiFormatBc4Snorm,
        DxgiFormatBc5Typeless,
        DxgiFormatBc5Unorm,
        DxgiFormatBc5Snorm,
        DxgiFormatB5G6R5Unorm,
        DxgiFormatB5G5R5A1Unorm,
        DxgiFormatB8G8R8A8Unorm,
        DxgiFormatB8G8R8X8Unorm,
        DxgiFormatR10G10B10XrBiasA2Unorm,
        DxgiFormatB8G8R8A8Typeless,
        DxgiFormatB8G8R8A8UnormSrgb,
        DxgiFormatB8G8R8X8Typeless,
        DxgiFormatB8G8R8X8UnormSrgb,
        DxgiFormatBc6HTypeless,
        DxgiFormatBc6HUf16,
        DxgiFormatBc6HSf16,
        DxgiFormatBc7Typeless,
        DxgiFormatBc7Unorm,
        DxgiFormatBc7UnormSrgb,
        DxgiFormatAyuv,
        DxgiFormatY410,
        DxgiFormatY416,
        DxgiFormatNv12,
        DxgiFormatP010,
        DxgiFormatP016,
        DxgiFormat420Opaque,
        DxgiFormatYuy2,
        DxgiFormatY210,
        DxgiFormatY216,
        DxgiFormatNv11,
        DxgiFormatAi44,
        DxgiFormatIa44,
        DxgiFormatP8,
        DxgiFormatA8P8,
        DxgiFormatB4G4R4A4Unorm,
        DxgiFormatP208,
        DxgiFormatV208,
        DxgiFormatV408,
        DxgiFormatForceUint,

        // Added here due to lack of an official value
        DxgiFormatAtcExt = 300,
        DxgiFormatAtcExplicitAlphaExt,
        DxgiFormatAtcInterpolatedAlphaExt
    };

    public static class DxgiFormatExtensions
    {
        public static int GetByteSize(this DxgiFormat format)
        {
            return format switch
            {
                DxgiFormat.DxgiFormatUnknown => 4,
                DxgiFormat.DxgiFormatR32G32B32A32Typeless => 4 * 4,
                DxgiFormat.DxgiFormatR32G32B32A32Float => 4 * 4,
                DxgiFormat.DxgiFormatR32G32B32A32Uint => 4 * 4,
                DxgiFormat.DxgiFormatR32G32B32A32Sint => 4 * 4,
                DxgiFormat.DxgiFormatR32G32B32Typeless => 4 * 3,
                DxgiFormat.DxgiFormatR32G32B32Float => 4 * 3,
                DxgiFormat.DxgiFormatR32G32B32Uint => 4 * 3,
                DxgiFormat.DxgiFormatR32G32B32Sint => 4 * 3,
                DxgiFormat.DxgiFormatR16G16B16A16Typeless => 4 * 2,
                DxgiFormat.DxgiFormatR16G16B16A16Float => 4 * 2,
                DxgiFormat.DxgiFormatR16G16B16A16Unorm => 4 * 2,
                DxgiFormat.DxgiFormatR16G16B16A16Uint => 4 * 2,
                DxgiFormat.DxgiFormatR16G16B16A16Snorm => 4 * 2,
                DxgiFormat.DxgiFormatR16G16B16A16Sint => 4 * 2,
                DxgiFormat.DxgiFormatR32G32Typeless => 4 * 2,
                DxgiFormat.DxgiFormatR32G32Float => 4 * 2,
                DxgiFormat.DxgiFormatR32G32Uint => 4 * 2,
                DxgiFormat.DxgiFormatR32G32Sint => 4 * 2,
                DxgiFormat.DxgiFormatR32G8X24Typeless => 4 * 2,
                DxgiFormat.DxgiFormatD32FloatS8X24Uint => 4,
                DxgiFormat.DxgiFormatR32FloatX8X24Typeless => 4,
                DxgiFormat.DxgiFormatX32TypelessG8X24Uint => 4,
                DxgiFormat.DxgiFormatR10G10B10A2Typeless => 4,
                DxgiFormat.DxgiFormatR10G10B10A2Unorm => 4,
                DxgiFormat.DxgiFormatR10G10B10A2Uint => 4,
                DxgiFormat.DxgiFormatR11G11B10Float => 4,
                DxgiFormat.DxgiFormatR8G8B8A8Typeless => 4,
                DxgiFormat.DxgiFormatR8G8B8A8Unorm => 4,
                DxgiFormat.DxgiFormatR8G8B8A8UnormSrgb => 4,
                DxgiFormat.DxgiFormatR8G8B8A8Uint => 4,
                DxgiFormat.DxgiFormatR8G8B8A8Snorm => 4,
                DxgiFormat.DxgiFormatR8G8B8A8Sint => 4,
                DxgiFormat.DxgiFormatR16G16Typeless => 4,
                DxgiFormat.DxgiFormatR16G16Float => 4,
                DxgiFormat.DxgiFormatR16G16Unorm => 4,
                DxgiFormat.DxgiFormatR16G16Uint => 4,
                DxgiFormat.DxgiFormatR16G16Snorm => 4,
                DxgiFormat.DxgiFormatR16G16Sint => 4,
                DxgiFormat.DxgiFormatR32Typeless => 4,
                DxgiFormat.DxgiFormatD32Float => 4,
                DxgiFormat.DxgiFormatR32Float => 4,
                DxgiFormat.DxgiFormatR32Uint => 4,
                DxgiFormat.DxgiFormatR32Sint => 4,
                DxgiFormat.DxgiFormatR24G8Typeless => 4,
                DxgiFormat.DxgiFormatD24UnormS8Uint => 4,
                DxgiFormat.DxgiFormatR24UnormX8Typeless => 4,
                DxgiFormat.DxgiFormatX24TypelessG8Uint => 4,
                DxgiFormat.DxgiFormatR8G8Typeless => 2,
                DxgiFormat.DxgiFormatR8G8Unorm => 2,
                DxgiFormat.DxgiFormatR8G8Uint => 2,
                DxgiFormat.DxgiFormatR8G8Snorm => 2,
                DxgiFormat.DxgiFormatR8G8Sint => 2,
                DxgiFormat.DxgiFormatR16Typeless => 2,
                DxgiFormat.DxgiFormatR16Float => 2,
                DxgiFormat.DxgiFormatD16Unorm => 2,
                DxgiFormat.DxgiFormatR16Unorm => 2,
                DxgiFormat.DxgiFormatR16Uint => 2,
                DxgiFormat.DxgiFormatR16Snorm => 2,
                DxgiFormat.DxgiFormatR16Sint => 2,
                DxgiFormat.DxgiFormatR8Typeless => 1,
                DxgiFormat.DxgiFormatR8Unorm => 1,
                DxgiFormat.DxgiFormatR8Uint => 1,
                DxgiFormat.DxgiFormatR8Snorm => 1,
                DxgiFormat.DxgiFormatR8Sint => 1,
                DxgiFormat.DxgiFormatA8Unorm => 1,
                DxgiFormat.DxgiFormatR1Unorm => 1,
                DxgiFormat.DxgiFormatR9G9B9E5Sharedexp => 4,
                DxgiFormat.DxgiFormatR8G8B8G8Unorm => 4,
                DxgiFormat.DxgiFormatG8R8G8B8Unorm => 4,
                DxgiFormat.DxgiFormatBc1Typeless => 8,
                DxgiFormat.DxgiFormatBc1Unorm => 8,
                DxgiFormat.DxgiFormatBc1UnormSrgb => 8,
                DxgiFormat.DxgiFormatBc2Typeless => 16,
                DxgiFormat.DxgiFormatBc2Unorm => 16,
                DxgiFormat.DxgiFormatBc2UnormSrgb => 16,
                DxgiFormat.DxgiFormatBc3Typeless => 16,
                DxgiFormat.DxgiFormatBc3Unorm => 16,
                DxgiFormat.DxgiFormatBc3UnormSrgb => 16,
                DxgiFormat.DxgiFormatBc4Typeless => 8,
                DxgiFormat.DxgiFormatBc4Unorm => 8,
                DxgiFormat.DxgiFormatBc4Snorm => 8,
                DxgiFormat.DxgiFormatBc5Typeless => 16,
                DxgiFormat.DxgiFormatBc5Unorm => 16,
                DxgiFormat.DxgiFormatBc5Snorm => 16,
                DxgiFormat.DxgiFormatB5G6R5Unorm => 2,
                DxgiFormat.DxgiFormatB5G5R5A1Unorm => 2,
                DxgiFormat.DxgiFormatB8G8R8A8Unorm => 4,
                DxgiFormat.DxgiFormatB8G8R8X8Unorm => 4,
                DxgiFormat.DxgiFormatR10G10B10XrBiasA2Unorm => 4,
                DxgiFormat.DxgiFormatB8G8R8A8Typeless => 4,
                DxgiFormat.DxgiFormatB8G8R8A8UnormSrgb => 4,
                DxgiFormat.DxgiFormatB8G8R8X8Typeless => 4,
                DxgiFormat.DxgiFormatB8G8R8X8UnormSrgb => 4,
                DxgiFormat.DxgiFormatBc6HTypeless => 16,
                DxgiFormat.DxgiFormatBc6HUf16 => 16,
                DxgiFormat.DxgiFormatBc6HSf16 => 16,
                DxgiFormat.DxgiFormatBc7Typeless => 16,
                DxgiFormat.DxgiFormatBc7Unorm => 16,
                DxgiFormat.DxgiFormatBc7UnormSrgb => 16,
                DxgiFormat.DxgiFormatP8 => 1,
                DxgiFormat.DxgiFormatA8P8 => 2,
                DxgiFormat.DxgiFormatB4G4R4A4Unorm => 2,
                DxgiFormat.DxgiFormatAtcExt => 8,
                DxgiFormat.DxgiFormatAtcExplicitAlphaExt => 16,
                DxgiFormat.DxgiFormatAtcInterpolatedAlphaExt => 16,
                _ => 4,
            };
        }

        public static bool IsCompressedFormat(this DxgiFormat format)
        {
            return format switch
            {
                DxgiFormat.DxgiFormatBc1Typeless or DxgiFormat.DxgiFormatBc1Unorm or DxgiFormat.DxgiFormatBc1UnormSrgb or DxgiFormat.DxgiFormatBc2Typeless or DxgiFormat.DxgiFormatBc2Unorm or DxgiFormat.DxgiFormatBc2UnormSrgb or DxgiFormat.DxgiFormatBc3Typeless or DxgiFormat.DxgiFormatBc3Unorm or DxgiFormat.DxgiFormatBc3UnormSrgb or DxgiFormat.DxgiFormatBc4Typeless or DxgiFormat.DxgiFormatBc4Unorm or DxgiFormat.DxgiFormatBc4Snorm or DxgiFormat.DxgiFormatBc5Typeless or DxgiFormat.DxgiFormatBc5Unorm or DxgiFormat.DxgiFormatBc5Snorm or DxgiFormat.DxgiFormatBc6HTypeless or DxgiFormat.DxgiFormatBc6HUf16 or DxgiFormat.DxgiFormatBc6HSf16 or DxgiFormat.DxgiFormatBc7Typeless or DxgiFormat.DxgiFormatBc7Unorm or DxgiFormat.DxgiFormatBc7UnormSrgb or DxgiFormat.DxgiFormatAtcExt or DxgiFormat.DxgiFormatAtcExplicitAlphaExt or DxgiFormat.DxgiFormatAtcInterpolatedAlphaExt => true,
                _ => false,
            };
        }
    }
}
