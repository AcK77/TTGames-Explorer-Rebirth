using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public struct NuFileTreeNode
    {
        public uint   ChildIndex;
        public uint   SiblingIndex;
        public string Name;
        public int    ParentIndex;
        public uint   FileIndex;
    }

    public class NuFileTree
    {
        public NuFileTreeNode[] Deserialize(BinaryReader reader)
        {
            uint nuFileTreeVersion = reader.ReadUInt32BigEndian();
            uint fileCount         = reader.ReadUInt32BigEndian();
            uint nodeCount         = reader.ReadUInt32BigEndian();
            uint leafNameSize      = reader.ReadUInt32BigEndian();

            long leafNamesPosition = reader.BaseStream.Position;

            NuFileTreeNode[] nodes = new NuFileTreeNode[nodeCount];

            reader.BaseStream.Seek(leafNameSize, SeekOrigin.Current);

            // Read nodes.

            if (nodeCount > 0)
            {
                for (int i = 0; i < nodeCount; i++)
                {
                    ushort childIndex   = reader.ReadUInt16BigEndian();
                    ushort siblingIndex = reader.ReadUInt16BigEndian();

                    string name = "";

                    if (nuFileTreeVersion == 1)
                    {
                        int nameHash = reader.ReadInt32BigEndian();

                        // TODO: Compute hash name and find it.
                    }
                    else
                    {
                        int nameOffset = reader.ReadInt32BigEndian();

                        long oldPosition = reader.BaseStream.Position;

                        if (nameOffset != -1)
                        {
                            reader.BaseStream.Seek(leafNamesPosition + nameOffset, SeekOrigin.Begin);

                            name = reader.ReadNullTerminatedString();

                            reader.BaseStream.Seek(oldPosition, SeekOrigin.Begin);
                        }
                    }

                    ushort parentIndex = reader.ReadUInt16BigEndian();
                    ushort fileIndex   = (ushort)((nuFileTreeVersion > 1) ? reader.ReadUInt16BigEndian() : i);

                    // TODO: Do the file path.
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
    }
#pragma warning restore IDE0059
}
