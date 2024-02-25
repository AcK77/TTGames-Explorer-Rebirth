namespace TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared
{
    /// <summary>
    /// The operation context.
    /// </summary>
    public class OperationContext
    {
        /// <summary>
        /// Whether the blocks should be decoded in parallel.
        /// </summary>
        public bool IsParallel { get; set; }

        /// <summary>
        /// Determines how many tasks should be used for parallel processing.
        /// </summary>
        public int TaskCount { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// The cancellation token to check if the asynchronous operation was cancelled.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// The progress context for the operation.
        /// </summary>
        public OperationProgress Progress { get; set; }
    }
}
