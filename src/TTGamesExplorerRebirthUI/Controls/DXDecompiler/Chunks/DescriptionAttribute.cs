namespace DXDecompiler.Chunks
{
    /// <summary>
    /// Provided here because Portable Class Libraries don't include
    /// System.ComponentModel.DescriptionAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DescriptionAttribute(string description, ChunkType chunkType = ChunkType.Unknown) : Attribute
    {
        private readonly string _description = description;
        private readonly ChunkType _chunkType = chunkType;

        public string Description
        {
            get { return _description; }
        }

        public ChunkType ChunkType
        {
            get { return _chunkType; }
        }
    }
}