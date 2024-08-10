using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuFileTreeNode
    {
        public NuFileTreeNode Child;
        public NuFileTreeNode Sibling;
        public string         Name;
        public NuFileTreeNode Parent;
        public int            FileIndex;

        public NuFileTreeNode Deserialize(BinaryReader reader, NuFileTreeNode[] nodes, uint nuFileTreeVersion, long leafNamesPosition)
        {
            uint childIndex = reader.ReadUInt16BigEndian();

            Child = nodes[(int)childIndex];

            uint siblingIndex = reader.ReadUInt16BigEndian();

            Sibling = nodes[(int)siblingIndex];

            Name = "";

            if (nuFileTreeVersion == 1)
            {
                int nameHash = reader.ReadInt32BigEndian();

                // TODO: Compute hash name and find it.
            }
            else
            {
                int  nameOffset  = reader.ReadInt32BigEndian();
                long oldPosition = reader.BaseStream.Position;

                if (nameOffset != -1)
                {
                    reader.BaseStream.Seek(leafNamesPosition + nameOffset, SeekOrigin.Begin);

                    Name = reader.ReadNullTerminatedString();

                    reader.BaseStream.Seek(oldPosition, SeekOrigin.Begin);
                }
            }

            int parentIndex = reader.ReadInt16BigEndian();
            if (parentIndex != -1)
            {
                Parent = nodes[parentIndex];
            }

            if (nuFileTreeVersion < 1)
            {
                FileIndex = -1;
            }
            else
            {
                short fileIndex = reader.ReadInt16BigEndian();

                FileIndex = childIndex == 0 ? fileIndex : -1;
            }

            return this;
        }

        public static string GetPath(NuFileTreeNode node)
        {
            string path = "";

            for (NuFileTreeNode iterator = node; iterator != null; iterator = iterator.Parent)
            {
                path = string.Format("{0}/{1}", iterator.Name, path);
            }

            return path.Substring(0, path.Length - 1);
        }
    }
#pragma warning restore IDE0059
}