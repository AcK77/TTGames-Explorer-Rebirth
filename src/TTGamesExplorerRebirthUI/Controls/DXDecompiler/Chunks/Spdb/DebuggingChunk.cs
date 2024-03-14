﻿using DXDecompiler.Util;

namespace DXDecompiler.Chunks.Spdb
{
    /// <summary>
    /// Contains debugging info in PDB format.
    /// </summary>
    public class DebuggingChunk : BytecodeChunk
    {
        /// <summary>
        /// Raw PDB bytes that can be read with a PDB parser.
        /// </summary>
        public byte[] PdbBytes { get; private set; }

        public static DebuggingChunk Parse(BytecodeReader reader, ChunkType chunkType, uint chunkSize)
        {
            var result = new DebuggingChunk();

            if(chunkType == ChunkType.Sdbg) // SDGB is not supported.
                return result;

            result.PdbBytes = reader.ReadBytes((int)chunkSize);
            return result;
        }
    }
}