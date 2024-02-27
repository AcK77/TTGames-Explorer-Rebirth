using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public static class NuFile
    {
        public const string MagicFourCC                      = ".CC4";
        public const string MagicResourceHeader              = "HSER";
        public const string MagicVirtualTableObjectReference = "ROTV";

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

            reader.ReadNuResourceVirtualTableObjectReference();

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
                    uint   nameOffset   = reader.ReadUInt32BigEndian();

                    long oldPosition = reader.BaseStream.Position;

                    reader.BaseStream.Seek(leafNamesPosition + nameOffset, SeekOrigin.Begin);

                    string name = reader.ReadNullTerminatedString();

                    reader.BaseStream.Seek(oldPosition, SeekOrigin.Begin);

                    ushort parentIndex = reader.ReadUInt16BigEndian();
                    ushort fileIndex   = reader.ReadUInt16BigEndian();

                    nodes[i] = new()
                    {
                        ChildIndex   = childIndex,
                        SiblingIndex = siblingIndex,
                        Name         = name,
                        ParentIndex  = parentIndex,
                        FileIndex    = fileIndex,
                    };
                }
            }

            return nodes;
        }

        public static void ReadNuResourceVirtualTableObjectReference(this BinaryReader reader)
        {
            if (reader.ReadUInt32AsString() != MagicVirtualTableObjectReference)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint size    = reader.ReadUInt32BigEndian();
            uint version = reader.ReadUInt32BigEndian();

            if (size > 0)
            {
                throw new NotSupportedException($"{reader.BaseStream.Position:x8}");
            }
        }
    }
#pragma warning restore IDE0059
}