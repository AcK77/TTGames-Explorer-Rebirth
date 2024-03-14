using DXDecompiler.Chunks;
using DXDecompiler.Chunks.Rdef;
using DXDecompiler.Decompiler.IR;
using DXDecompiler.Decompiler.IR.ResourceDefinitions;
using System.Text;

namespace DXDecompiler.Decompiler.Writer
{
    public class ResourceDefinitionWriter(DecompileContext context) : BaseWriter(context)
    {
        static bool EmitRegisterDeclarations => true;
        public static bool EmitPackingOffset => true;
        public static bool MergeStructDefinitions => false;

        public void WriteResourceDefinition(IrResourceDefinition resourceDefinition)
        {
            HashSet<string> seen = [];
            WriteLine("// ConstantBuffers");
            WriteLine(resourceDefinition.ConstantBuffersDebug);
            foreach (var constantBuffer in resourceDefinition.ConstantBuffers)
            {
                WriteConstantBuffer(resourceDefinition, constantBuffer, ref seen);
            }
            WriteLine("// ResourceBindings");
            WriteLine(resourceDefinition.ResourceBindingsDebug);
            foreach (var resourceBinding in resourceDefinition.ResourceBindings)
            {
                WriteResourceBinding(resourceBinding);
            }
            WriteLine();
        }

        private void WriteResourceBinding(IrResourceBinding binding)
        {
            if (binding.Kind == IrResourceKind.CBuffer || binding.Kind == IrResourceKind.TBuffer)
            {
                WriteLineFormat("//{0} {1}", binding.Kind, binding.Name);
                return;
            }
            var typeName = GetBindingTypeName(binding);
            WriteFormat("{0}", typeName);
            if (binding.ReturnType != null)
            {
                var dimStr = binding.Dimension > 0 ? $", {binding.Dimension}" : "";
                if (binding.ReturnType.VariableClass == ShaderVariableClass.Struct)
                {
                    WriteFormat("<struct {0}>", binding.ReturnType.BaseTypeName);
                }
                else
                {
                    WriteFormat("<{0}{1}>", GetShaderTypeDeclaration(binding.ReturnType), dimStr);
                }
            }
            WriteFormat(" {0}", binding.Name);
            if (binding.BindCount > 0)
            {
                WriteFormat("[{0}]", binding.BindCount);
            }
            if (EmitRegisterDeclarations)
            {
                WriteFormat(" : register({0})", binding.GetBindPointDescription());
            }
            WriteLine(";");
        }

        static string GetBindingTypeName(IrResourceBinding binding)
        {
            return binding.Class switch
            {
                IrResourceClass.CBuffer => "cbuffer",
                IrResourceClass.Sampler => "SamplerState",
                IrResourceClass.SRV => binding.Kind switch
                {
                    IrResourceKind.Texture1D => "Texture1D",
                    IrResourceKind.Texture2D => "Texture2D",
                    IrResourceKind.Texture2DMS => "Texture2DMS",
                    IrResourceKind.Texture3D => "Texture3D",
                    IrResourceKind.TextureCube => "TextureCube",
                    IrResourceKind.Texture1DArray => "Texture1DArray",
                    IrResourceKind.Texture2DArray => "Texture2DArray",
                    IrResourceKind.Texture2DMSArray => "Texture2DMSArray",
                    IrResourceKind.TextureCubeArray => "TextureCubeArray",
                    IrResourceKind.TypedBuffer => "Buffer",
                    IrResourceKind.RawBuffer => "ByteAddressBuffer",
                    IrResourceKind.StructuredBuffer => "StructuredBuffer",
                    _ => $"Invalid_{binding.Class}_{binding.Kind}",
                },
                IrResourceClass.UAV => binding.Kind switch
                {
                    IrResourceKind.Texture1D => "RWTexture1D",
                    IrResourceKind.Texture2D => "RWTexture2D",
                    IrResourceKind.Texture2DMS => "RWTexture2DMS",
                    IrResourceKind.Texture3D => "RWTexture3D",
                    IrResourceKind.TextureCube => "RWTextureCube",
                    IrResourceKind.Texture1DArray => "RWTexture1DArray",
                    IrResourceKind.Texture2DArray => "RWTexture2DArray",
                    IrResourceKind.Texture2DMSArray => "RWTexture2DMSArray",
                    IrResourceKind.TextureCubeArray => "RWTextureCubeArray",
                    IrResourceKind.TypedBuffer => "RWBuffer",
                    IrResourceKind.RawBuffer => "RWByteAddressBuffer",
                    IrResourceKind.StructuredBuffer => "RWStructuredBuffer",
                    _ => $"Invalid_{binding.Class}_{binding.Kind}",
                },
                _ => $"Invalid_{binding.Class}_{binding.Kind}",
            };
        }

        void WriteConstantBuffer(IrResourceDefinition rdef, IrConstantBuffer constantBuffer, ref HashSet<string> seen)
        {
            if (!string.IsNullOrEmpty(constantBuffer.Debug))
            {
                WriteLineFormat("// {0}", constantBuffer.Debug);
            }
            if (constantBuffer.BufferType == ConstantBufferType.ConstantBuffer ||
                constantBuffer.BufferType == ConstantBufferType.TextureBuffer)
            {
                if (constantBuffer.Name == "$Globals")
                {
                    WriteLine("// Globals");
                    foreach (var variable in constantBuffer.Variables)
                    {
                        WriteVariable(variable, 0, packOffset: false);
                    }
                    WriteLine();
                }
                else if (constantBuffer.Name == "$Params")
                {
                }
                else
                {
                    var resourceBinding = rdef.ResourceBindings.FirstOrDefault(rb =>
                        (rb.Kind == IrResourceKind.CBuffer || rb.Kind == IrResourceKind.TBuffer) &&
                        rb.Name == constantBuffer.Name);
                    var bufferName = resourceBinding.Kind == IrResourceKind.CBuffer ?
                        "cbuffer" :
                        "tbuffer";
                    Write($"{bufferName} {constantBuffer.Name}");
                    if (EmitRegisterDeclarations)
                    {
                        var registerName = resourceBinding.Kind == IrResourceKind.CBuffer ?
                            "b" :
                            "t";
                        Write($" : register({registerName}{resourceBinding.BindPoint})");
                    }
                    WriteLine();
                    WriteLine("{");
                    foreach (var variable in constantBuffer.Variables)
                    {
                        WriteVariable(variable);
                    }
                    WriteLine("}");
                    WriteLine("");
                }
            }
            else if (constantBuffer.BufferType == ConstantBufferType.ResourceBindInformation)
            {
                WriteLineFormat("// Resource bind info for {0}", constantBuffer.Name);
                var element = constantBuffer.Variables.Single();
                var baseType = element.ShaderType;
                if (baseType.VariableClass != ShaderVariableClass.Struct)
                {
                    Write("// ");
                    WriteVariable(element);
                    return;
                }
                var typeName = element.ShaderType.BaseTypeName;
                if (string.IsNullOrEmpty(typeName))
                {
                    var index = rdef.ConstantBuffers
                        .Where(cb => cb.BufferType == ConstantBufferType.ResourceBindInformation)
                        .ToList()
                        .IndexOf(constantBuffer);
                    typeName = $"struct{index}";
                }

#pragma warning disable CA1868
                if (seen.Contains(typeName))
                {
                    WriteLine($"// {baseType} {typeName}");
                }
                else
                {
                    seen.Add(typeName);
                    WriteLine(GetShaderTypeDeclaration(baseType, overrideName: typeName));
                }
#pragma warning restore CA1868
            }
        }

        void WriteVariable(IrShaderVariable variable, int indentLevel = 1, bool packOffset = true)
        {
            Write($"{GetShaderTypeDeclaration(variable.ShaderType, indentLevel, root: false)} {variable.Name}");
            if (variable.ShaderType.ElementCount > 0)
            {
                WriteFormat("[{0}]", variable.ShaderType.ElementCount);
            }
            if (variable.DefaultValue != null)
            {
                if (variable.DefaultValue.Count > 1)
                {
                    WriteFormat(" = {0}({1})",
                        GetShaderTypeDeclaration(variable.ShaderType),
                        string.Join(", ", variable.DefaultValue));
                }
                else
                {
                    WriteFormat(" = {0}", variable.DefaultValue[0]);
                }
            }
            // packoffset needs to be disabled for globals
            if (EmitPackingOffset && packOffset)
            {
                var componentOffset = variable.StartOffset % 16 / 4;
                string componentPacking = "";
                switch (componentOffset)
                {
                    case 1:
                        componentPacking = ".y";
                        break;
                    case 2:
                        componentPacking = ".z";
                        break;
                    case 3:
                        componentPacking = ".w";
                        break;
                }
                WriteFormat(" : packoffset(c{0}{1})", variable.StartOffset / 16, componentPacking);
            }
            Write($"; // Offset {variable.StartOffset} Size {variable.Size} CBSize {variable.Member.GetCBVarSize(true)}");
            if (variable.Flags.HasFlag(ShaderVariableFlags.Used))
            {
                Write($" [unused]");
            }
            WriteLine();
        }

        string GetShaderTypeDeclaration(IrShaderType type, int indent = 0, string overrideName = null, bool root = true)
        {
            var sb = new StringBuilder();
            string indentString = new(' ', indent * 4);
            string baseTypeName = overrideName ?? type.BaseTypeName;
            switch (type.VariableClass)
            {
                case ShaderVariableClass.InterfacePointer:
                    sb.Append(indentString);
                    sb.Append(string.Format("{0}{1}", type.VariableClass.GetDescription(), type.BaseTypeName));
                    break;
                case ShaderVariableClass.MatrixColumns:
                case ShaderVariableClass.MatrixRows:
                    {
                        sb.Append(indentString);
                        sb.Append(type.VariableClass.GetDescription());
                        sb.Append(type.VariableType.GetVariableTypeName());
                        sb.Append(type.Rows);
                        sb.Append("x" + type.Columns);
                        break;
                    }
                case ShaderVariableClass.Vector:
                    {
                        sb.Append(indentString + type.VariableType.GetVariableTypeName());
                        sb.Append(type.Columns);
                        break;
                    }
                case ShaderVariableClass.Struct:
                    {
                        //SM4 doesn't include typenames, check by signature
                        //TODO
                        if (!MergeStructDefinitions || root || baseTypeName == null || baseTypeName.EndsWith("<unnamed>"))
                        {
                            sb.Append(indentString + "struct ");
                            if (baseTypeName == null || baseTypeName.EndsWith("<unnamed>"))
                            {
                            }
                            else
                            {
                                sb.Append(baseTypeName);
                            }
                            sb.AppendLine("");
                            sb.AppendLine(indentString + "{");
                            foreach (var member in type.Members)
                                sb.AppendLine(GetShaderMemberDeclaration(member, indent + 1));
                            sb.Append(indentString + "}");
                        }
                        else
                        {
                            sb.Append(indentString + "struct " + baseTypeName); //struct keyword optional
                        }
                        break;
                    }
                case ShaderVariableClass.Scalar:
                    {
                        sb.Append(indentString + type.VariableType.GetVariableTypeName());
                        break;
                    }
                default:
                    throw new InvalidOperationException(string.Format("Variable class '{0}' is not currently supported.", type.VariableClass));
            }
            return sb.ToString();
        }

        string GetShaderMemberDeclaration(IrShaderTypeMember member, int indent)
        {
            string declaration = GetShaderTypeDeclaration(member.Type, indent, root: false) + " " + member.Name;
            if (member.Type.ElementCount > 0)
                declaration += string.Format("[{0}]", member.Type.ElementCount);
            declaration += ";";
            declaration += $" // Offset {member.Offset} CBSize {member.GetCBVarSize(true)}";
            return declaration;
        }
    }
}