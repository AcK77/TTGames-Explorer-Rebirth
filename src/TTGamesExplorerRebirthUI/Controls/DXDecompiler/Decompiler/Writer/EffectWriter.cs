using DXDecompiler.Decompiler.IR;

namespace DXDecompiler.Decompiler.Writer
{
    public class EffectWriter(DecompileContext context) : BaseWriter(context)
    {
        public void WriteEffect(IrEffect effect)
        {
            foreach(var group in effect.EffectChunk.Groups)
            {
                Write(group.ToString());
            }
        }
    }
}