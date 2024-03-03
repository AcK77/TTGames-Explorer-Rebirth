using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuFileTree
    {
        public List<NuFileTreeNode> Nodes { get; private set; }

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
                Nodes = [];

                for (uint i = 0; i < nodeCount; i++)
                {
                    Nodes.Add(new NuFileTreeNode().Deserialize(reader, nuFileTreeVersion, leafNamesPosition, i));
                }

                uint unknown1 = reader.ReadUInt32BigEndian();
            }

            return this;
        }
    }
#pragma warning restore IDE0059
}