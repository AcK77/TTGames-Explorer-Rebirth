namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public struct NuResourceHeader
    {
        public uint   Version;
        public uint   Type;
        public string ProjectName;
        public string ProducedByUserName;
        public string SourceFileName;

        public FileTreeNode[] Nodes;
    }

    public struct FileTreeNode
    {
        public uint   ChildIndex;
        public uint   SiblingIndex;
        public string Name;
        public uint   ParentIndex;
        public uint   FileIndex;
    }
}