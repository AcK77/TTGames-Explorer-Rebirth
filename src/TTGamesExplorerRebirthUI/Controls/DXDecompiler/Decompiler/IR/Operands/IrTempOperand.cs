namespace DXDecompiler.Decompiler.IR.Operands
{
    public class IrTempOperand(uint index) : IrOperand
    {
        public uint Index = index;
    }
}