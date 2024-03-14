using DXDecompiler.Chunks;
using DXDecompiler.Chunks.Common;
using DXDecompiler.Chunks.Xsgn;
using DXDecompiler.Decompiler.IR;

namespace DXDecompiler.Decompiler.Writer
{
    public class SignatureWriter(DecompileContext context) : BaseWriter(context)
    {
        public void WriteSignature(IrSignature signature)
        {
            WriteIndent();
            WriteLineFormat("struct {0} {{", signature.Name);
            IncreaseIndent();
            foreach (var param in signature.Chunk.Parameters)
            {
                WriteParameter(param);
            }
            DecreaseIndent();
            WriteLine("};");
        }

        void WriteParameter(SignatureParameterDescription param)
        {
            WriteIndent();
            var fieldType = GetFieldType(param);
            Write($"{fieldType} {param.GetName()} : {GetSemanticName(param)};");
            DebugSignatureParamater(param);
        }

        void DebugSignatureParamater(SignatureParameterDescription param)
        {
            Write(" // ");
            WriteLine(param.ToString());
        }

        static string GetFieldType(SignatureParameterDescription param)
        {
            string fieldType = param.ComponentType.GetDescription();
            if (param.MinPrecision != MinPrecision.None)
            {
                fieldType = param.MinPrecision.GetTypeName();
            }
            int componentCount = 0;
            if (param.Mask.HasFlag(ComponentMask.X)) componentCount += 1;
            if (param.Mask.HasFlag(ComponentMask.Y)) componentCount += 1;
            if (param.Mask.HasFlag(ComponentMask.Z)) componentCount += 1;
            if (param.Mask.HasFlag(ComponentMask.W)) componentCount += 1;
            return componentCount switch
            {
                1 => $"{fieldType}",
                2 => $"{fieldType}2",
                3 => $"{fieldType}3",
                4 => $"{fieldType}4",
                _ => throw new Exception($"Invalid ComponentMask {param.Mask}"),
            };
        }

        static string GetSemanticName(SignatureParameterDescription param)
        {
            if (param.SemanticName == "TEXCOORD")
            {
                return $"{param.SemanticName}{param.SemanticIndex}";
            }
            return param.SemanticName;
        }
    }
}