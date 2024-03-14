using DXDecompiler.Chunks.Common;
using DXDecompiler.Util;
using System.Text;

namespace DXDecompiler.Chunks.Xsgn
{
    public abstract class InputOutputSignatureChunk : BytecodeChunk
    {
        public SignatureParameterDescriptionCollection Parameters { get; private set; }

        protected InputOutputSignatureChunk()
        {
            Parameters = [];
        }

        public static InputOutputSignatureChunk Parse(BytecodeReader reader, ChunkType chunkType)
        {
            InputOutputSignatureChunk result = chunkType switch
            {
                ChunkType.Isgn or ChunkType.Isg1 => new InputSignatureChunk(),
                ChunkType.Osgn or ChunkType.Osg1 or ChunkType.Osg5 => new OutputSignatureChunk(),
                ChunkType.Pcsg or ChunkType.Psg1 => new PatchConstantSignatureChunk(),
                _ => throw new ArgumentOutOfRangeException(nameof(chunkType), "Unrecognised chunk type: " + chunkType),
            };

            var chunkReader = reader.CopyAtCurrentPosition();
            var elementCount = chunkReader.ReadUInt32();
            _ /*uniqueKey*/ = chunkReader.ReadUInt32();
            var elementSize = chunkType switch
            {
                ChunkType.Isgn or ChunkType.Osgn or ChunkType.Pcsg => SignatureElementSize._6,
                ChunkType.Osg5 => SignatureElementSize._7,
                ChunkType.Osg1 or ChunkType.Isg1 or ChunkType.Psg1 => SignatureElementSize._8,
                _ => throw new ArgumentOutOfRangeException(nameof(chunkType), "Unrecognised chunk type: " + chunkType),
            };

            for (int i = 0; i < elementCount; i++)
                result.Parameters.Add(SignatureParameterDescription.Parse(reader, chunkReader, chunkType, elementSize));

            return result;
        }

        internal void UpdateVersion(ShaderVersion version)
        {
            foreach (var param in Parameters)
            {
                param.UpdateVersion(ChunkType, version);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("// Name                 Index   Mask Register SysValue  Format   Used");
            sb.AppendLine("// -------------------- ----- ------ -------- -------- ------- ------");
            bool includeStreams = Parameters.Any(p => p.Stream > 0);
            foreach (var parameter in Parameters)
                sb.AppendLine("// " + parameter.ToString(includeStreams));

            if (Parameters.Any())
                sb.AppendLine("//");

            return sb.ToString();
        }
    }
}