namespace DXDecompiler.Decompiler.IR.Operands
{
    public class IrOutputOperand(uint index, uint rowIndex, uint colIndex) : IrOperand
    {
        public uint Index = index;
        public uint RowIndex = rowIndex;
        public uint ColIndex = colIndex;
    }
}