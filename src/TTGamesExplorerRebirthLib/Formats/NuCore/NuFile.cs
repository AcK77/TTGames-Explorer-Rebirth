using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public static class NuFile
    {
        public const string MagicFourCC         = ".CC4";
        public const string MagicResourceHeader = "HSER";
        public const string MagicVector         = "ROTV";

        public static uint ReadNuFileHeader(this BinaryReader reader)
        {
            uint nuResourceHeaderSize = reader.ReadUInt32BigEndian();

            if (reader.ReadUInt32AsString() != MagicFourCC)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            return nuResourceHeaderSize;
        }

        public static NuResourceHeader ReadNuResourceHeader(this BinaryReader reader)
        {
            if (reader.ReadUInt32AsString() != MagicResourceHeader)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            if (reader.ReadUInt32AsString() != MagicResourceHeader)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            NuResourceHeader nuResourceHeader = new()
            {
                Version = reader.ReadUInt32BigEndian(),
                Type    = reader.ReadUInt32BigEndian(),
            };

            if (nuResourceHeader.Type == 1)
            {
                nuResourceHeader.Nodes = reader.ReadNuFileTree();
            }

            reader.ReadNuVector(nuResourceHeader);

            nuResourceHeader.ProjectName = reader.ReadSized16NullTerminatedString();

            uint resourceType       = reader.ReadUInt32();
            uint accurevTransaction = reader.ReadUInt32();

            nuResourceHeader.ProducedByUserName = reader.ReadSized16NullTerminatedString();

            sbyte unknown1 = reader.ReadSByte();

            nuResourceHeader.SourceFileName = reader.ReadSized16NullTerminatedString();

            sbyte unknown2 = reader.ReadSByte();

            return nuResourceHeader;
        }

        public static FileTreeNode[] ReadNuFileTree(this BinaryReader reader)
        {
            uint nuFileTreeVersion = reader.ReadUInt32BigEndian();
            uint fileCount         = reader.ReadUInt32BigEndian();
            uint nodeCount         = reader.ReadUInt32BigEndian();
            uint leafNameSize      = reader.ReadUInt32BigEndian();

            long leafNamesPosition = reader.BaseStream.Position;

            FileTreeNode[] nodes = new FileTreeNode[nodeCount];

            reader.BaseStream.Seek(leafNameSize, SeekOrigin.Current);

            // Read nodes.

            if (nodeCount > 0)
            {
                for (int i = 0; i < nodeCount; i++)
                {
                    ushort childIndex   = reader.ReadUInt16BigEndian();
                    ushort siblingIndex = reader.ReadUInt16BigEndian();
                    int    nameOffset   = reader.ReadInt32BigEndian();

                    long oldPosition = reader.BaseStream.Position;

                    string name = "";

                    if (nameOffset != -1)
                    {
                        reader.BaseStream.Seek(leafNamesPosition + nameOffset, SeekOrigin.Begin);

                        name = reader.ReadNullTerminatedString();

                        reader.BaseStream.Seek(oldPosition, SeekOrigin.Begin);
                    }

                    ushort parentIndex = reader.ReadUInt16BigEndian();
                    ushort fileIndex   = (ushort)((nuFileTreeVersion > 1) ? reader.ReadUInt16BigEndian() : i);

                    nodes[i] = new()
                    {
                        ChildIndex   = childIndex,
                        SiblingIndex = siblingIndex,
                        Name         = name,
                        ParentIndex  = parentIndex,
                        FileIndex    = fileIndex,
                    };
                }

                uint unknown1 = reader.ReadUInt32BigEndian();
            }

            return nodes;
        }

        public static void ReadNuVector(this BinaryReader reader, NuResourceHeader nuResourceHeader)
        {
            if (reader.ReadUInt32AsString() != MagicVector)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint size = reader.ReadUInt32BigEndian();
            uint id   = reader.ReadUInt32BigEndian();

            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    uint type = reader.ReadUInt32BigEndian();

                    if (nuResourceHeader.Version <= 6)
                    {
                        uint oldParam = reader.ReadUInt32BigEndian();
                    }

                    ulong hash = reader.ReadUInt32BigEndian();
                    if (hash == 1)
                    {
                        uint unknown = reader.ReadUInt32BigEndian();
                        byte[] nuChecksum = reader.ReadBytes(0x10);
                    }
                    else
                    {
                        uint unknown = reader.ReadUInt32BigEndian();
                    }

                    if (nuResourceHeader.Version >= 3)
                    {
                        uint platformsAndClasses = reader.ReadUInt32BigEndian();

                        if (nuResourceHeader.Version >= 6)
                        {
                            uint forContext = reader.ReadUInt32BigEndian();
                            uint withContext = reader.ReadUInt32BigEndian();
                        }

                        if (nuResourceHeader.Version >= 8)
                        {
                            uint discipline = reader.ReadUInt32BigEndian();
                        }
                    }
                }
            }
        }
    }
#pragma warning restore IDE0059
}