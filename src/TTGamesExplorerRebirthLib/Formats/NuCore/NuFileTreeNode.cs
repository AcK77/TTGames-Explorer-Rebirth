using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuFileTreeNode
    {
        public uint   ChildIndex;
        public uint   SiblingIndex;
        public string Name;
        public int    ParentIndex;
        public uint   FileIndex;

        public NuFileTreeNode Deserialize(BinaryReader reader, uint nuFileTreeVersion, long leafNamesPosition, uint index)
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
                int  nameOffset  = reader.ReadInt32BigEndian();
                long oldPosition = reader.BaseStream.Position;

                if (nameOffset != -1)
                {
                    reader.BaseStream.Seek(leafNamesPosition + nameOffset, SeekOrigin.Begin);

                    name = reader.ReadNullTerminatedString();

                    reader.BaseStream.Seek(oldPosition, SeekOrigin.Begin);
                }
            }

            ushort parentIndex = reader.ReadUInt16BigEndian();
            ushort fileIndex   = (ushort)((nuFileTreeVersion > 1) ? reader.ReadUInt16BigEndian() : index);

            // TODO: Do the file path.

            return this;
        }
    }
#pragma warning restore IDE0059
}