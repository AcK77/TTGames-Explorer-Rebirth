using TTGamesExplorerRebirthLib.Hash;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.DAT
{
    /// <summary>
    ///     Give DAT file path and get a list of contained files through "Files" field.
    ///     
    ///     Files can be extracted using "ExtractFile" method.
    /// </summary>
    /// <remarks>
    ///     Based on QuickBMS script by Luigi Auriemma:
    ///     https://aluigi.altervista.org/quickbms.htm
    ///     
    ///     Based on my own research (Ac_K).
    ///     
    ///     Games supported:
    ///         - LEGO The Lord of the Rings
    ///         - LEGO Star Wars - The Complete Saga
    ///         - LEGO Worlds
    ///
    /// </remarks>
    public class DATArchive
    {
        public const string MagicCC40TAD = ".CC40TAD";

        public List<DATFile> Files = [];

        public string ArchiveFilePath;

        private struct TempFile
        {
            public ulong             Offset;
            public uint              Size;
            public uint              CompressedSize;
            public CompressionFormat Compression;
        }

        // TODO: Improve the deserializer.
        public DATArchive(string archiveFilePath)
        {
            ArchiveFilePath = archiveFilePath;

            using FileStream   stream = new(archiveFilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(stream);

            // Read DAT header.

            uint infoTableOffset = reader.ReadUInt32();

            if ((infoTableOffset & 0x80000000) != 0)
            {
                infoTableOffset ^= 0xFFFFFFFF;
                infoTableOffset <<= 8;
                infoTableOffset += 0x100;
            }

            uint infoTableSize = reader.ReadUInt32();

            stream.Seek(infoTableOffset, SeekOrigin.Begin);

            uint versionType1 = reader.ReadUInt32();
            uint versionType2 = reader.ReadUInt32();

            stream.Seek(infoTableOffset, SeekOrigin.Begin);

            // TODO: Improve this check.
            if (versionType1.ToConvertedString() == "4CC." || 
                versionType1.ToConvertedString() == ".CC4" || 
                versionType2.ToConvertedString() == "4CC." || 
                versionType2.ToConvertedString() == ".CC4")
            {
                // Read DAT info table.

                uint datInfoTableSize = reader.ReadUInt32();

                if (reader.ReadUInt64AsString() != MagicCC40TAD)
                {
                    throw new InvalidDataException($"{stream.Position:x8}");
                }

                int   formatByteOrder1 = reader.ReadInt32BigEndian(); // -11
                uint  formatVersion    = reader.ReadUInt32BigEndian();
                uint  filesCount1      = reader.ReadUInt32BigEndian();
                uint  namesCount       = reader.ReadUInt32BigEndian();
                uint  namesSize        = reader.ReadUInt32BigEndian();
                ulong namesOffset      = (ulong)stream.Position;

                stream.Seek(namesSize, SeekOrigin.Current);

                ushort unknown2 = reader.ReadUInt16BigEndian(); // 0x1000
                ushort unknown3 = reader.ReadUInt16BigEndian(); // -1

                // Read file names table.

                uint lastNameWorkaround = namesCount - 1;
                ushort myId = 0;

                string[] namesTemp = new string[namesCount];
                string[] namesList = new string[filesCount1];

                for (int i = 0; i < namesCount; i++)
                {
                    uint   nameOffset = reader.ReadUInt32BigEndian();
                    ushort folderId   = reader.ReadUInt16BigEndian();
                    ushort unknown1Id = reader.ReadUInt16BigEndian();
                    ushort unknown2Id = reader.ReadUInt16BigEndian();
                    ushort fileId     = reader.ReadUInt16BigEndian();

                    if (nameOffset != 0xFFFFFFFF)
                    {
                        long latestOffset = stream.Position;

                        stream.Seek((long)namesOffset + nameOffset, SeekOrigin.Begin);

                        string fileName = stream.ReadNullTerminatedString();

                        stream.Seek(latestOffset, SeekOrigin.Begin);

                        // TODO: Find a proper solution.
                        if (i == lastNameWorkaround)
                        {
                            fileId = myId;
                        }

                        fileName = $"{namesTemp[folderId]}\\{fileName}";

                        if (fileId != 0)
                        {
                            namesList[myId] = fileName;
                            myId++;
                        }
                        else
                        {
                            namesTemp[i] = fileName;
                        }
                    }
                }

                // Read files info table.

                int  formatByteOrder2 = reader.ReadInt32BigEndian(); // -11 / Version ?
                uint filesCount2      = reader.ReadUInt32BigEndian();

                if (filesCount2 != filesCount1)
                {
                    throw new IndexOutOfRangeException($"{stream.Position:x8}");
                }

                TempFile[] tempFilesList = new TempFile[filesCount2];

                for (int i = 0; i < filesCount2; i++)
                {
                    tempFilesList[i].Offset = (formatByteOrder2 <= -11) ? reader.ReadUInt64BigEndian() : reader.ReadUInt32BigEndian();
                    
                    uint compressedSize = reader.ReadUInt32BigEndian();
                    uint size           = reader.ReadUInt32BigEndian();

                    // TODO: Rework this once more format versions will be added.
                    uint compressed;
                    if (formatByteOrder2 <= -11)
                    {
                        compressed   = size;
                        size        &= 0x7FFFFFFF;
                        compressed >>= 31;
                    }
                    else
                    {
                        compressed = reader.ReadByte();

                        ushort zero = reader.ReadUInt16();

                        tempFilesList[i].Offset |= (uint)(reader.ReadByte() << 8);
                    }

                    tempFilesList[i].Size           = size;
                    tempFilesList[i].CompressedSize = compressedSize;
                    tempFilesList[i].Compression    = GetFileCompressionFormat(stream, reader, tempFilesList[i].Offset);
                }

                // Read FNV1a table.

                uint[] fnv1aFilesList = new uint[filesCount2];
                for (int i = 0; i < filesCount2; i++)
                {
                    fnv1aFilesList[i] = reader.ReadUInt32BigEndian();
                }

                // Compute FNV1a of generated names path.

                uint[] fnv1aNamesList = GenerateFnv1aNamesList(namesList, filesCount1);

                // Merge all infos.

                AddToFilesList(filesCount2, fnv1aFilesList, fnv1aNamesList, namesList, tempFilesList);
            }
            else
            {
                // Read DAT info table.

                int  formatByteOrder1 = reader.ReadInt32(); // LSTTCC > -3 - LLOTR > -5 / Version ?
                uint filesCount1      = reader.ReadUInt32();

                     infoTableOffset = (uint)stream.Position;
                uint nameInfoOffset  = infoTableOffset + filesCount1 * 0x10;

                stream.Seek(nameInfoOffset, SeekOrigin.Begin);

                uint namesCount     = reader.ReadUInt32();
                     nameInfoOffset = (uint)stream.Position;
                uint nameFieldSize  = (uint)((formatByteOrder1 <= -5) ? 12 : 8);
                uint namesOffset    = nameInfoOffset + (namesCount * nameFieldSize);

                stream.Seek(namesOffset, SeekOrigin.Begin);

                uint namesCrcOffset  = reader.ReadUInt32();
                     namesOffset     = (uint)stream.Position;
                     namesCrcOffset += (uint)stream.Position;

                stream.Seek(namesCrcOffset, SeekOrigin.Begin);

                ulong fnv1aTableOffset = (ulong)stream.Position;

                // Read FNV1a filenames table.

                uint[] fnv1aFilesList = new uint[filesCount1];

                for (int i = 0; i < fnv1aFilesList.Length; i++)
                {
                    fnv1aFilesList[i] = reader.ReadUInt32();
                }

                // Read files infos.

                if (formatByteOrder1 <= -2)
                {
                    reader.ReadInt32();
                    reader.ReadInt32();

                    // TODO: We should be at EOF.
                    if (stream.Position != stream.Length)
                    {
                        throw new IndexOutOfRangeException($"{stream.Position:x8}");
                    }
                }

                uint     nameIndex = 0;
                string   fullName  = "";
                string   fullPath  = "";
                string[] namesList = new string[filesCount1];
                string[] tempArray = new string[ushort.MaxValue]; // TODO: Find a better way to handle that.

                for (int i = 0; i < filesCount1; i++)
                {
                    short  next = 1;
                    string name = "";

                    do
                    {
                        stream.Seek(nameInfoOffset, SeekOrigin.Begin);

                              next   = reader.ReadInt16();
                        short prev   = reader.ReadInt16();
                        int   offset = reader.ReadInt32();

                        if (formatByteOrder1 <= -5) // if (nameFieldSize >= 12)
                        {
                            reader.ReadUInt32();
                        }

                        nameInfoOffset = (uint)stream.Position;

                        if (offset > 0)
                        {
                            offset += (int)namesOffset;

                            stream.Seek(offset, SeekOrigin.Begin);

                            name = stream.ReadNullTerminatedString();
                        }

                        // NOTE: Used only for LEGO the game if you don't use the hdr file.
                        if (name.Length > 0)
                        {
                            if (name[0] >= 0xf0)
                            {
                                name = "";
                            }
                        }

                        if (prev != 0)
                        {
                            fullPath = tempArray[prev];
                        }

                        tempArray[(int)nameIndex] = fullPath;

                        if (next > 0) // Folder
                        {
                            string tempName = tempArray[prev];

                            if (tempName != "") // NOTE: Long story to avoid things like 2foldername that gives problems.
                            {
                                string oldName = @"\"; // Do not use "/"
                                oldName += tempName;
                                oldName += @"\"; // Do not use "/"
                            }

                            if (name != "")
                            {
                                fullPath += name;
                                fullPath += @"\"; // Do not use "/"
                            }
                        }

                        nameIndex += 1;
                    } while (next > 0);

                    fullName = fullPath;
                    fullName += name;

                    namesList[i] = $"\\{fullName.ToLowerInvariant()}";
                }

                TempFile[] tempFilesList = new TempFile[filesCount1];

                for (int i = 0; i < filesCount1; i++)
                {
                    stream.Seek(infoTableOffset + i * 0x10, SeekOrigin.Begin);

                    uint   offsetFile     = reader.ReadUInt32();
                    uint   compressedSize = reader.ReadUInt32();
                    uint   size           = reader.ReadUInt32();
                    byte[] packed         = reader.ReadBytes(3);
                    byte   offsetFile2    = reader.ReadByte();

                    if (formatByteOrder1 != -1)
                    {
                        offsetFile <<= 8;
                    }

                    offsetFile += offsetFile2;

                    tempFilesList[i].Offset         = offsetFile;
                    tempFilesList[i].Size           = size;
                    tempFilesList[i].CompressedSize = compressedSize;
                    tempFilesList[i].Compression    = GetFileCompressionFormat(stream, reader, tempFilesList[i].Offset);
                }

                // Compute FNV1a of generated names path.

                uint[] fnv1aNamesList = GenerateFnv1aNamesList(namesList, filesCount1);

                // Merge all infos.

                AddToFilesList(filesCount1, fnv1aFilesList, fnv1aNamesList, namesList, tempFilesList);
            }
        }

        private static uint[] GenerateFnv1aNamesList(string[] namesList, uint filesCount)
        {
            uint[] fnv1aNamesList = new uint[filesCount];

            for (int i = 0; i < fnv1aNamesList.Length; i++)
            {
                fnv1aNamesList[i] = Fnv.Fnv1a_32_TTGames(namesList[i][1..]);
            }

            return fnv1aNamesList;
        }

        private static CompressionFormat GetFileCompressionFormat(FileStream stream, BinaryReader reader, ulong offset)
        {
            long oldPosition = stream.Position;

            stream.Seek((long)offset, SeekOrigin.Begin);

            // TODO: Found a better way to know if files are compressed/encrypted.
            //       The byte packed in the size field ("compressed" var) is only available for LZ2K/ZIPX.

            CompressionFormat compressionFormat = reader.ReadUInt32AsString() switch
            {
                "LZ2K" => CompressionFormat.LZ2K,
                "ZIPX" => CompressionFormat.ZIPX,
                "Defl" => CompressionFormat.Deflate_v1,
                _ => CompressionFormat.None,
            };

            stream.Seek(oldPosition, SeekOrigin.Begin);

            return compressionFormat;
        }

        private void AddToFilesList(uint filesCount, uint[] fnv1aFilesList, uint[] fnv1aNamesList, string[] namesList, TempFile[] tempFilesList)
        {
            for (int i = 0; i < filesCount; i++)
            {
                int j = Array.IndexOf(fnv1aFilesList, fnv1aNamesList[i]);

                Files.Add(new DATFile(namesList[i], tempFilesList[j].Offset, tempFilesList[j].Size, tempFilesList[j].CompressedSize, tempFilesList[j].Compression));
            }
        }

        /// <summary>
        ///     Extract DATFile from the DAT archive.
        /// </summary>
        /// <param name="file">
        ///     DATFile object representing the file to be extracted.
        /// </param>
        /// <param name="plainData">
        ///     Bool to decrypt/decompress the file while it's extracted or not.
        /// </param>
        /// <returns>
        ///     Byte array representing the file data.
        /// </returns>
        public byte[] ExtractFile(DATFile file, bool plainData = false)
        {
            using FileStream stream = new(ArchiveFilePath, FileMode.Open, FileAccess.Read);

            stream.Seek((long)file.Offset, SeekOrigin.Begin);

            uint   size     = file.Compression == CompressionFormat.None ? file.Size : file.CompressedSize;
            byte[] fileData = new byte[size];

            stream.Read(fileData, 0, (int)size);

            if (plainData)
            {
                switch (file.Compression)
                {
                    case CompressionFormat.LZ2K:
                        {
                            fileData = LZ2K.Decompress(fileData);
                            break;
                        }

                    case CompressionFormat.ZIPX:
                        {
                            fileData = ZIPX.Decrypt(fileData);
                            break;
                        }

                    case CompressionFormat.Deflate_v1:
                        {
                            fileData = Deflatev1.Decompress(fileData);
                            break;
                        }
                }
            }

            return fileData;
        }
    }
}