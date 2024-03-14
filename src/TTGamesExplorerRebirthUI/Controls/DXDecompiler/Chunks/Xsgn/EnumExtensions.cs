using DXDecompiler.Chunks.Common;
using DXDecompiler.Chunks.Shex;

namespace DXDecompiler.Chunks.Xsgn
{
    public static class EnumExtensions
    {
        public static string GetDescription(this ComponentMask value)
        {
            string result = string.Empty;

            result += (value.HasFlag(ComponentMask.X)) ? "x" : " ";
            result += (value.HasFlag(ComponentMask.Y)) ? "y" : " ";
            result += (value.HasFlag(ComponentMask.Z)) ? "z" : " ";
            result += (value.HasFlag(ComponentMask.W)) ? "w" : " ";

            return result;
        }

        public static bool RequiresMask(this Name value)
        {
            return value switch
            {
                Name.Coverage or Name.Depth or Name.DepthGreaterEqual or Name.DepthLessEqual or Name.StencilRef => false,
                _ => true,
            };
        }

        public static string GetRegisterName(this Name value)
        {
            return value switch
            {
                Name.DepthGreaterEqual => OperandType.OutputDepthGreaterEqual.GetDescription(),
                Name.DepthLessEqual => OperandType.OutputDepthLessEqual.GetDescription(),
                Name.Coverage => OperandType.OutputCoverageMask.GetDescription(),
                Name.Depth => OperandType.OutputDepth.GetDescription(),
                Name.StencilRef => OperandType.StencilRef.GetDescription(),
                Name.PrimitiveID => "primID",
                _ => throw new ArgumentOutOfRangeException(nameof(value), "Unrecognised name: " + value),
            };
        }
    }
}