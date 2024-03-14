using DXDecompiler.Chunks.Shex.Tokens;
using DXDecompiler.Decompiler.IR.Operands;

namespace DXDecompiler.Decompiler.DxbcParser
{
    public class OperandParser
    {
        public static IrOperand Parse(Operand operand)
        {
            IrOperand result = operand.OperandType switch
            {
                Chunks.Shex.OperandType.ConstantBuffer => new IrConstantBufferOperand(0, 0, 0),
                Chunks.Shex.OperandType.Sampler => new IrResourceOperand(IR.IrResourceType.Sampler, 0),
                Chunks.Shex.OperandType.Resource => new IrResourceOperand(IR.IrResourceType.Texture, 0),
                Chunks.Shex.OperandType.UnorderedAccessView => new IrResourceOperand(IR.IrResourceType.UnorderedAccessView, 0),
                _ => new IrDebugOperand(operand.ToString()),
            };

            result.DebugText = operand.ToString();

            return result;
        }
    }
}