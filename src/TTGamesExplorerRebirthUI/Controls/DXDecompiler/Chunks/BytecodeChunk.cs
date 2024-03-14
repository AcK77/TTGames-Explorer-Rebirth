using DXDecompiler.Chunks.Aon9;
using DXDecompiler.Chunks.Dxil;
using DXDecompiler.Chunks.Fx10;
using DXDecompiler.Chunks.Fxlvm;
using DXDecompiler.Chunks.Hash;
using DXDecompiler.Chunks.Ifce;
using DXDecompiler.Chunks.Ildb;
using DXDecompiler.Chunks.Libf;
using DXDecompiler.Chunks.Priv;
using DXDecompiler.Chunks.Psv0;
using DXDecompiler.Chunks.Rdat;
using DXDecompiler.Chunks.Rdef;
using DXDecompiler.Chunks.RTS0;
using DXDecompiler.Chunks.Sfi0;
using DXDecompiler.Chunks.Shex;
using DXDecompiler.Chunks.Spdb;
using DXDecompiler.Chunks.Stat;
using DXDecompiler.Chunks.Xsgn;
using DXDecompiler.Util;

namespace DXDecompiler.Chunks
{
    public abstract class BytecodeChunk
    {
        private static readonly Dictionary<uint, ChunkType> KnownChunkTypes = new()
        {
            { "IFCE".ToFourCc(), ChunkType.Ifce },
            { "ISGN".ToFourCc(), ChunkType.Isgn },
            { "OSGN".ToFourCc(), ChunkType.Osgn },
            { "OSG5".ToFourCc(), ChunkType.Osg5 },
            { "PCSG".ToFourCc(), ChunkType.Pcsg },
            { "PSG1".ToFourCc(), ChunkType.Psg1 },
            { "RDEF".ToFourCc(), ChunkType.Rdef },
            { "SDBG".ToFourCc(), ChunkType.Sdbg },
            { "SFI0".ToFourCc(), ChunkType.Sfi0 },
            { "SHDR".ToFourCc(), ChunkType.Shdr },
            { "SHEX".ToFourCc(), ChunkType.Shex },
            { "SPDB".ToFourCc(), ChunkType.Spdb },
            { "STAT".ToFourCc(), ChunkType.Stat },
            { "ISG1".ToFourCc(), ChunkType.Isg1 },
            { "OSG1".ToFourCc(), ChunkType.Osg1 },
            { "Aon9".ToFourCc(), ChunkType.Aon9 },
            { "XNAS".ToFourCc(), ChunkType.Xnas },
            { "XNAP".ToFourCc(), ChunkType.Xnap },
            { "PRIV".ToFourCc(), ChunkType.Priv },
            { "RTS0".ToFourCc(), ChunkType.Rts0 },
            { "LIBF".ToFourCc(), ChunkType.Libf },
            { "LIBH".ToFourCc(), ChunkType.Libh },
            { "LFS0".ToFourCc(), ChunkType.Lfs0 },
            { "FX10".ToFourCc(), ChunkType.Fx10 },
            { "CTAB".ToFourCc(), ChunkType.Ctab },
            { "CLI4".ToFourCc(), ChunkType.Cli4 },
            { "FXLC".ToFourCc(), ChunkType.Fxlc },
            { "DXIL".ToFourCc(), ChunkType.Dxil },
            { "HASH".ToFourCc(), ChunkType.Hash },
            { "PSV0".ToFourCc(), ChunkType.Psv0 },
            { "RDAT".ToFourCc(), ChunkType.Rdat },
            { "ILDB".ToFourCc(), ChunkType.Ildb },
            { "ILDN".ToFourCc(), ChunkType.Ildn }
        };

        public BytecodeContainer Container { get; private set; }
        public uint FourCc { get; private set; }
        public ChunkType ChunkType { get; private set; }
        public uint ChunkSize { get; private set; }

        public static BytecodeChunk ParseChunk(BytecodeReader chunkReader, BytecodeContainer container)
        {
            // Type of chunk this is.
            uint fourCc = chunkReader.ReadUInt32();

            // Total length of the chunk in bytes.
            uint chunkSize = chunkReader.ReadUInt32();

            ChunkType chunkType;
            if (KnownChunkTypes.TryGetValue(fourCc, out ChunkType value))
            {
                chunkType = value;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "Chunk type '" + fourCc.ToFourCcString() + "' is not yet supported.");
                System.Diagnostics.Debug.WriteLine("Chunk type '" + fourCc.ToFourCcString() + "' is not yet supported.");
                return null;
            }

            var chunkContentReader = chunkReader.CopyAtCurrentPosition((int)chunkSize);
            BytecodeChunk chunk = chunkType switch
            {
                ChunkType.Ifce => InterfacesChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Isgn or ChunkType.Osgn or ChunkType.Osg5 or ChunkType.Pcsg or ChunkType.Isg1 or ChunkType.Osg1 or ChunkType.Psg1 => InputOutputSignatureChunk.Parse(chunkContentReader, chunkType),
                ChunkType.Rdef => ResourceDefinitionChunk.Parse(chunkContentReader),
                ChunkType.Sdbg or ChunkType.Spdb => DebuggingChunk.Parse(chunkContentReader, chunkType, chunkSize),
                ChunkType.Sfi0 => Sfi0Chunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Shdr or ChunkType.Shex => ShaderProgramChunk.Parse(chunkContentReader),
                ChunkType.Stat => StatisticsChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Xnas or ChunkType.Xnap or ChunkType.Aon9 => Level9ShaderChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Priv => PrivateChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Rts0 => RootSignatureChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Libf => LibfChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Libh => LibHeaderChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Lfs0 => LibraryParameterSignatureChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Fx10 => EffectChunk.Parse(chunkContentReader),
                ChunkType.Ctab => CtabChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Cli4 => Cli4Chunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Fxlc => FxlcChunk.Parse(chunkContentReader, chunkSize, container),
                ChunkType.Dxil => DxilChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Hash => HashChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Psv0 => PipelineStateValidationChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Rdat => RuntimeDataChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Ildb => DebugInfoDXILChunk.Parse(chunkContentReader, chunkSize),
                ChunkType.Ildn => DebugNameChunk.Parse(chunkContentReader, chunkSize),
                _ => throw new ParseException("Invalid chunk type: " + chunkType),
            };
            chunk.Container = container;
            chunk.FourCc = fourCc;
            chunk.ChunkSize = chunkSize;
            chunk.ChunkType = chunkType;

            return chunk;
        }
    }
}