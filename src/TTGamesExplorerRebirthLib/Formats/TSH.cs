using System.Text;
using TTGamesExplorerRebirthLib.Formats.DDS;
using TTGamesExplorerRebirthLib.Formats.NuCore;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    public class TSHEntry
    {
        public float MinU;
        public float MinV;
        public float MaxU;
        public float MaxV;

        public uint MinX;
        public uint MinY;
        public uint Width;
        public uint Height;

        public string Magic;

        public int TrimTop;
        public int TrimBottom;
        public int TrimLeft;
        public int TrimRight;

        public uint NameHash; // FNV132 hash in uppercase (_content.txt file for the list)
    }

    /// <summary>
    ///     Give tsh file (NuTextureSheet) data and deserialize it.
    /// </summary>
    /// <remarks>
    ///     Based on my own research (Ac_K).
    ///     
    ///     Some field names found by Jay Franco.
    /// </remarks>
    public class TSH
    {
        private const string MagicTxSh = "HSXT";

        public NuResourceHeader Header;

        public DDSImage Image;

        public List<TSHEntry> Entries = [];

        public TSH(string archiveFilePath)
        {
            Deserialize(File.ReadAllBytes(archiveFilePath));
        }

        public TSH(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            // Read header.

            reader.ReadNuFileHeader();

            Header = reader.ReadNuResourceHeader();

            // Read NuTextureSheetHeader.

            uint nuTextureSetHeaderSize = reader.ReadNuFileHeader();

            if (reader.ReadUInt32AsString() != MagicTxSh)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicTxSh)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint nuTextureSetHeaderVersion = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != NuFile.MagicVector)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint nuTextureInfoCount = reader.ReadUInt32BigEndian();

            // Read entries.

            for (int i = 0; i < nuTextureInfoCount; i++)
            {
                TSHEntry entry = new()
                {
                    MinU = reader.ReadSingleBigEndian(),
                    MinV = reader.ReadSingleBigEndian(),
                    MaxU = reader.ReadSingleBigEndian(),
                    MaxV = reader.ReadSingleBigEndian(),

                    MinX   = reader.ReadUInt32BigEndian(),
                    MinY   = reader.ReadUInt32BigEndian(),
                    Width  = reader.ReadUInt32BigEndian(),
                    Height = reader.ReadUInt32BigEndian(),

                    Magic = Encoding.ASCII.GetString(reader.ReadBytes(4)),

                    TrimTop    = reader.ReadInt32BigEndian(),
                    TrimBottom = reader.ReadInt32BigEndian(),
                    TrimLeft   = reader.ReadInt32BigEndian(),
                    TrimRight  = reader.ReadInt32BigEndian(),

                    NameHash = reader.ReadUInt32()
                };

                Entries.Add(entry);
            }

            Image = new DDSImage(reader.ReadBytes((int)(stream.Length - (int)stream.Position)));
        }
    }
}