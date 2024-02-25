namespace TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared
{
    public struct ProgressElement
    {
        /// <summary>
        /// Current block being processed
        /// </summary>
        public int CurrentBlock { get; }

        /// <summary>
        /// The total amount of blocks to be processed
        /// </summary>
        public int TotalBlocks { get; }

        /// <summary>
        /// Returns the progress percentage as a float from 0 to 1
        /// </summary>
        public readonly float Percentage => CurrentBlock / (float)TotalBlocks;

        public ProgressElement(int currentBlock, int totalBlocks)
        {
            CurrentBlock = currentBlock;
            TotalBlocks = totalBlocks;
        }

        public override readonly string ToString()
        {
            return $"{nameof(CurrentBlock)}: {CurrentBlock}, {nameof(TotalBlocks)}: {TotalBlocks}, {nameof(Percentage)}: {Percentage}";
        }
    }
}
