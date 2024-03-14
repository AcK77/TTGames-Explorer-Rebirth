using DXDecompiler.Chunks.Xsgn;

namespace DXDecompiler.Decompiler.IR
{
    public class IrSignature(InputOutputSignatureChunk chunk, string name, IrSignatureType signatureType)
    {
        internal InputOutputSignatureChunk Chunk = chunk;
        public string Name = name;
        public IrSignatureType SignatureType = signatureType;
    }
}