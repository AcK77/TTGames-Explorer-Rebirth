using DXDecompiler.Chunks;
using DXDecompiler.Chunks.Common;
using DXDecompiler.Chunks.Dxil;
using DXDecompiler.Chunks.Fx10;
using DXDecompiler.Chunks.Ifce;
using DXDecompiler.Chunks.Libf;
using DXDecompiler.Chunks.Psv0;
using DXDecompiler.Chunks.Rdef;
using DXDecompiler.Chunks.Sfi0;
using DXDecompiler.Chunks.Shex;
using DXDecompiler.Chunks.Shex.Tokens;
using DXDecompiler.Chunks.Stat;
using DXDecompiler.Chunks.Xsgn;
using DXDecompiler.Util;
using System.Text;

namespace DXDecompiler
{
    public class BytecodeContainer
    {
        private readonly byte[] _rawBytes;

        public BytecodeContainerHeader Header { get; private set; }
        public List<BytecodeChunk> Chunks { get; private set; }

        public ResourceDefinitionChunk ResourceDefinition
        {
            get { return Chunks.OfType<ResourceDefinitionChunk>().SingleOrDefault(); }
        }

        public PatchConstantSignatureChunk PatchConstantSignature
        {
            get { return Chunks.OfType<PatchConstantSignatureChunk>().SingleOrDefault(); }
        }

        public InputSignatureChunk InputSignature
        {
            get { return Chunks.OfType<InputSignatureChunk>().SingleOrDefault(); }
        }

        public OutputSignatureChunk OutputSignature
        {
            get { return Chunks.OfType<OutputSignatureChunk>().SingleOrDefault(); }
        }

        public Sfi0Chunk Sfi0
        {
            get { return Chunks.OfType<Sfi0Chunk>().SingleOrDefault(); }
        }

        public LibraryParameterSignatureChunk LibrarySignature
        {
            get { return Chunks.OfType<LibraryParameterSignatureChunk>().SingleOrDefault(); }
        }

        public ShaderProgramChunk Shader
        {
            get { return Chunks.OfType<ShaderProgramChunk>().SingleOrDefault(); }
        }

        public T GetChunk<T>()
        {
            return Chunks.OfType<T>().SingleOrDefault();
        }

        public StatisticsChunk Statistics
        {
            get { return Chunks.OfType<StatisticsChunk>().SingleOrDefault(); }
        }

        public InterfacesChunk Interfaces
        {
            get { return Chunks.OfType<InterfacesChunk>().SingleOrDefault(); }
        }

        /// <summary>
        /// Version is stored in both Resource Definition and Shader chunks.
        /// Generally the resource definition chunk is the first chunk in the container
        /// but in the case of library linked functions, it can be near the end. 
        /// Version is needed for parsing certain chunks, such as input/output
        /// </summary>
        public ShaderVersion Version
        {
            get
            {
                if (ResourceDefinition != null)
                {
                    return ResourceDefinition.Target;
                }

                if (Shader != null)
                {
                    return Shader.Version;
                }

                if (Chunks.OfType<LibfChunk>().Any())
                {
                    return Chunks.OfType<LibfChunk>().First().LibraryContainer.Version;
                }

                if (Chunks.OfType<EffectChunk>().Any())
                {
                    return Chunks.OfType<EffectChunk>().First().Header.Version;
                }

                if (Chunks.OfType<DxilChunk>().Any())
                {
                    return Chunks.OfType<DxilChunk>().First().Version;
                }

                return null;
            }
        }

        public BytecodeContainer(byte[] rawBytes)
        {
            _rawBytes = rawBytes;
            Chunks = [];

            var reader = new BytecodeReader(rawBytes, 0, rawBytes.Length);
            var magicNumber = BitConverter.ToUInt32(rawBytes, 0);
            if (magicNumber == 0xFEFF2001)
            {
                Chunks.Add(EffectChunk.Parse(reader));

                return;
            }

            Header = BytecodeContainerHeader.Parse(reader);

            for (uint i = 0; i < Header.ChunkCount; i++)
            {
                uint chunkOffset = reader.ReadUInt32();
                var chunkReader = reader.CopyAtOffset((int)chunkOffset);

                var chunk = BytecodeChunk.ParseChunk(chunkReader, this);
                if (chunk != null)
                {
                    Chunks.Add(chunk);
                }
            }

            foreach (var chunk in Chunks.OfType<InputOutputSignatureChunk>())
            {
                chunk.UpdateVersion(Version);
            }

            foreach (var chunk in Chunks.OfType<Sfi0Chunk>())
            {
                chunk.UpdateVersion(Version);
            }

            foreach (var chunk in Chunks.OfType<PipelineStateValidationChunk>())
            {
                chunk.UpdateVersion(Version);
            }
        }

        internal BytecodeContainer()
        {
        }

        public static BytecodeContainer Parse(byte[] bytes)
        {
            return new BytecodeContainer(bytes);
        }

        private void WriteLibShader(StringBuilder sb)
        {
            sb.AppendLine("//");
            var libHeader = Chunks.OfType<LibHeaderChunk>().Single();
            sb.Append(libHeader.ToString());
            sb.AppendLine("//");
            if (Chunks.OfType<LibfChunk>().Any())
            {
                foreach (var chunk in Chunks.OfType<LibfChunk>())
                {
                    sb.Append(chunk.ToString());
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Chunks.OfType<LibfChunk>().Any())
            {
                WriteLibShader(sb);

                return sb.ToString();
            }

            if (Chunks.OfType<EffectChunk>().Any())
            {
                foreach (var chunk in Chunks.OfType<EffectChunk>())
                {
                    sb.AppendLine(chunk.ToString());
                }

                foreach (var chunk in Chunks.Where(c => c is not EffectChunk))
                {
                    sb.AppendLine($"{chunk.ChunkType} {chunk.GetType()}");
                }

                return sb.ToString();
            }

            sb.AppendLine("//");
            sb.AppendLine("// Generated by " + ResourceDefinition?.Creator ?? "Unknown");
            sb.AppendLine("//");
            sb.AppendLine("//");

            if (Shader != null && Shader.Tokens.Any(x => x.Header.OpcodeType == OpcodeType.Abort)) // TODO
            {
                sb.AppendLine("// Note: SHADER WILL ONLY WORK WITH THE DEBUG SDK LAYER ENABLED.");
                sb.AppendLine("//");
                sb.AppendLine("//");
            }

            if (Sfi0 != null)
            {
                sb.Append(Sfi0);
            }
            else if (Version.MajorVersion >= 5)
            {
                var globalFlagsDcl = Shader.DeclarationTokens
                    .OfType<GlobalFlagsDeclarationToken>()
                    .FirstOrDefault();

                if (globalFlagsDcl != null)
                {
                    sb.Append(Sfi0Chunk.GlobalFlagsToString(globalFlagsDcl?.Flags ?? 0));
                }
            }

            if (ResourceDefinition != null)
            {
                sb.Append(ResourceDefinition);
            }

            sb.AppendLine(@"//");

            if (PatchConstantSignature != null)
            {
                sb.Append(PatchConstantSignature);
                sb.AppendLine(@"//");
            }

            sb.Append(InputSignature);
            sb.AppendLine(@"//");

            sb.Append(OutputSignature);

            if (LibrarySignature != null)
            {
                sb.Append(LibrarySignature);
                sb.AppendLine(@"//");
            }

            if (Version.MajorVersion <= 5)
            {
                sb.Append(Statistics);
            }

            if (Interfaces != null)
            {
                sb.Append(Interfaces);
            }

            foreach (var dx9Shader in Chunks.OfType<Chunks.Aon9.Level9ShaderChunk>().OrderBy(chunk => chunk.ChunkType))
            {
                sb.AppendLine("//");
                sb.Append(dx9Shader);
            }

            if (Shader != null)
            {
                sb.Append(Shader);
            }

            if (Statistics != null)
            {
                sb.AppendFormat("// Approximately {0} instruction slots used", Statistics.InstructionCount);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static implicit operator byte[](BytecodeContainer container)
        {
            return container._rawBytes;
        }
    }
}