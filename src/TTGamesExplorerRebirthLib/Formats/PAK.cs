using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    public class PAKFile
    {
        public string Name;
        public uint   Offset;
        public uint   Size;
        public byte[] Data;
    }

    /// <summary>
    ///     Give pak file data and deserialize it.
    /// </summary>
    /// <remarks>
    ///     Based on my own research (Ac_K).
    /// </remarks>
    public class PAK
    {
        private const uint Magic = 0x1234567A;

        public List<PAKFile> Files = [];

        public PAK(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public PAK(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            // Read header.

            if (reader.ReadUInt32() != Magic)
            {
                throw new InvalidDataException($"{stream.Position:x8}");
            }

            uint fileCount   = reader.ReadUInt32();
            uint archiveSize = reader.ReadUInt32();
            uint checksum    = reader.ReadUInt32();
            uint unknown1    = reader.ReadUInt32(); // Always 0.
            uint unknown2    = reader.ReadUInt32(); // TODO: Known values: 0x00402F4C.

            // NOTE: Checksum is computed over the whole file with the field "checksum" zeroed.
            //       It uses TTGamesChecksum.PAK(buffer);

            // Iterate over each file info.

            for (int i = 0; i < fileCount; i++)
            {
                // Read file info.

                uint nameOffset   = reader.ReadUInt32();
                uint fileOffset   = reader.ReadUInt32();
                uint fileSize     = reader.ReadUInt32();
                uint flag         = reader.ReadUInt32(); // TODO: Filetype ? (0 > Cubemap Texture, 1 > Text, 4 > Texture, 10 > Animation
                uint unknownFile1 = reader.ReadUInt32(); // Always 0.
                uint unknownHash1 = reader.ReadUInt32(); // TODO: Determine what and how it's hashed here. (File name? File data?)
                uint unknownHash2 = reader.ReadUInt32(); // TODO: Determine what and how it's hashed here. (File name? File data?).

                // Read file name.

                long oldPosition = stream.Position;

                stream.Seek(nameOffset, SeekOrigin.Begin);

                string fileName = reader.ReadNullTerminatedString().ToLowerInvariant();

                stream.Seek(oldPosition, SeekOrigin.Begin);

                Files.Add(new()
                {
                    Name   = fileName,
                    Offset = fileOffset,
                    Size   = fileSize,
                });
            }

            for (int i = 0; i < fileCount; i++)
            {
                // Read file data.

                stream.Seek(Files[i].Offset, SeekOrigin.Begin);

                Files[i].Data = reader.ReadBytes((int)Files[i].Size);
            }
        }
    }
}