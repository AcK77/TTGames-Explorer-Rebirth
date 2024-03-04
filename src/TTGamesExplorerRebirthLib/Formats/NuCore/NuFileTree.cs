using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuFileTree
    {
        public string[] Files { get; private set; }

        public NuFileTree Deserialize(BinaryReader reader)
        {
            uint nuFileTreeVersion = reader.ReadUInt32BigEndian();
            uint fileCount         = reader.ReadUInt32BigEndian();
            uint nodeCount         = reader.ReadUInt32BigEndian();
            uint leafNameSize      = reader.ReadUInt32BigEndian();
            long leafNamesPosition = reader.BaseStream.Position;

            reader.BaseStream.Seek(leafNameSize, SeekOrigin.Current);

            if (nodeCount > 0)
            {
                NuFileTreeNode[] nodes = new NuFileTreeNode[nodeCount];

                for (uint i = 0; i < nodeCount; i++)
                {
                    nodes[i] = new NuFileTreeNode().Deserialize(reader, nodes, nuFileTreeVersion, leafNamesPosition);
                }

                Files = new string[fileCount];

                for (int i = 0; i < nodeCount; i++)
                {
                    if (nodes[i].FileIndex != -1)
                    {
                        Files[nodes[i].FileIndex] = NuFileTreeNode.GetPath(nodes[i]);
                    }
                }

                uint unknown1 = reader.ReadUInt32BigEndian();
            }

            return this;
        }
    }
#pragma warning restore IDE0059
}