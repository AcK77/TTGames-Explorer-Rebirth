using DXDecompiler.Chunks.Shex.Tokens;

namespace DXDecompiler.Decompiler.IR
{
    public class IrPass(string name, IrPass.PassType type)
    {
        public enum PassType
        {
            PixelShader,
            VertexShader,
            GeometryShader,
            DomainShader,
            ComputeShader,
            FunctionBody,
            HullShaderControlPointPhase,
            HullShaderPatchConstantPhase,
            HullShaderForkPhase,
            HullShaderJoinPhase,
        }

        internal List<DeclarationToken> Declarations = [];
        public List<IrInstruction> Instructions = [];
        public List<IrAttribute> Attributes = [];
        public string Name = name;
        public PassType Type = type;
        public IrSignature InputSignature;
        public IrSignature OutputSignature;
    }
}