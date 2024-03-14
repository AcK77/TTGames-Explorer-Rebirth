using DXDecompiler.Chunks;
using DXDecompiler.Chunks.Common;
using DXDecompiler.Chunks.Rdef;
using DXDecompiler.Chunks.Shex;
using DXDecompiler.Chunks.Shex.Tokens;
using System.Text;

namespace DXDecompiler.Decompiler
{
    public partial class OldHLSLDecompiler
    {
        /*
         * This should be merged with Declrations
         * Resource Definitions and InputOutput Signatures have corresponding declration tokens
         */
        void WriteResoureDefinitions()
        {
            var rdef = Container.ResourceDefinition;
            HashSet<string> seen = [];
            Output.AppendLine("// ConstantBuffers");
            WriteCBInnerStructs(ref seen);
            foreach (var constantBuffer in rdef.ConstantBuffers)
            {
                WriteConstantBuffer(constantBuffer, ref seen);
            }

            Output.AppendLine("// ResourceBindings");
            foreach (var resourceBinding in rdef.ResourceBindings)
            {
                WriteResourceBinding(resourceBinding);
            }
            Output.AppendLine();
        }
        void WriteCBInnerStructs(ref HashSet<string> seen)
        {
            foreach (var cb in Container.ResourceDefinition.ConstantBuffers)
            {
                foreach (var variable in cb.Variables)
                {
                    WriteCBInnerStructs(variable.Member, ref seen);
                }
            }
        }
        void WriteCBInnerStructs(ShaderTypeMember member, ref HashSet<string> seen)
        {
            if (member.Type.VariableClass != ShaderVariableClass.Struct) return;
            if (seen.Contains(member.Type.BaseTypeName)) return;
            foreach (var child in member.Type.Members)
            {
                WriteCBInnerStructs(child, ref seen);
            }
            if (member.Type.BaseTypeName == null || member.Type.BaseTypeName.EndsWith("<unnamed>")) return;
            seen.Add(member.Type.BaseTypeName);
            Output.AppendFormat("{0};\n", GetShaderTypeDeclaration(member.Type));
        }
        void WriteConstantBuffer(ConstantBuffer constantBuffer, ref HashSet<string> seen)
        {
            if (constantBuffer.BufferType == ConstantBufferType.ConstantBuffer ||
                constantBuffer.BufferType == ConstantBufferType.TextureBuffer)
            {
                if (constantBuffer.Name == "$Globals")
                {
                    Output.AppendLine("// Globals");
                    foreach (var variable in constantBuffer.Variables)
                    {
                        WriteVariable(variable, 0, packOffset: false);
                    }
                    Output.AppendLine("");
                }
                else if (constantBuffer.Name == "$Params")
                {
                }
                else
                {
                    var resourceBinding = Container.ResourceDefinition.ResourceBindings.FirstOrDefault(rb =>
                        (rb.Type == ShaderInputType.CBuffer || rb.Type == ShaderInputType.TBuffer) &&
                        rb.Name == constantBuffer.Name);
                    var bufferName = resourceBinding.Type == ShaderInputType.CBuffer ?
                        "cbuffer" :
                        "tbuffer";
                    Output.Append($"{bufferName} {constantBuffer.Name}");
                    if (EmitRegisterDeclarations)
                    {
                        var registerName = resourceBinding.Type == ShaderInputType.CBuffer ?
                            "b" :
                            "t";
                        Output.Append($" : register({registerName}{resourceBinding.BindPoint})");
                    }
                    Output.AppendLine("");
                    Output.AppendLine("{");
                    foreach (var variable in constantBuffer.Variables)
                    {
                        WriteVariable(variable);
                    }
                    Output.AppendLine("}");
                    Output.AppendLine("");
                }
            }
            else if (constantBuffer.BufferType == ConstantBufferType.ResourceBindInformation)
            {
                Output.AppendLine("// ResourceBindInformation");
                var element = constantBuffer.Variables.Single();
                var baseType = element.ShaderType;
                if (baseType.VariableClass != ShaderVariableClass.Struct)
                {
                    Output.AppendLine(element.ToString());
                    return;
                }
                var typeName = element.ShaderType.BaseTypeName;
                if (string.IsNullOrEmpty(typeName))
                {
                    var index = Container.ResourceDefinition.ConstantBuffers
                        .Where(cb => cb.BufferType == ConstantBufferType.ResourceBindInformation)
                        .ToList()
                        .IndexOf(constantBuffer);
                    typeName = $"struct{index}";
                }
#pragma warning disable CA1868
                if (seen.Contains(typeName))
                {
                    Output.AppendLine($"// {baseType} {typeName}");
                }
                else
                {
                    seen.Add(typeName);
                    Output.AppendLine(GetShaderTypeDeclaration(baseType, overrideName: typeName));
                }
#pragma warning restore CA1868
            }
        }

        static string GetTextureTypeName(ResourceBinding resourceBinding)
        {
            return resourceBinding.Dimension switch
            {
                ShaderResourceViewDimension.Texture1D => "Texture1D",
                ShaderResourceViewDimension.Texture1DArray => "Texture1DArray",
                ShaderResourceViewDimension.Texture2D => "Texture2D",
                ShaderResourceViewDimension.Texture2DArray => "Texture2DArray",
                ShaderResourceViewDimension.Buffer => "Buffer",
                ShaderResourceViewDimension.Texture2DMultiSampled => "Texture2DMS",
                ShaderResourceViewDimension.Texture2DMultiSampledArray => "Texture2DMSArray",
                ShaderResourceViewDimension.TextureCube => "TextureCube",
                ShaderResourceViewDimension.TextureCubeArray => "TextureCubeArray",
                ShaderResourceViewDimension.Texture3D => "Texture3D",
                _ => throw new ArgumentException($"Unexpected resource binding  dimension type {resourceBinding.Dimension}"),
            };
        }

        static string GetUAVTypeName(ResourceBinding resourceBinding)
        {
            return resourceBinding.Dimension switch
            {
                ShaderResourceViewDimension.Buffer => "RWBuffer",
                ShaderResourceViewDimension.Texture1D => "RWTexture1D",
                ShaderResourceViewDimension.Texture1DArray => "RWTexture1DArray",
                ShaderResourceViewDimension.Texture2D => "RWTexture2D",
                ShaderResourceViewDimension.Texture2DArray => "RWTexture2DArray",
                ShaderResourceViewDimension.Texture3D => "RWTexture3D",
                _ => throw new ArgumentException($"Unexpected resource binding dimension type {resourceBinding.Dimension}"),
            };
        }

        static string GetBindingName(ResourceBinding resourceBinding)
        {
            return resourceBinding.Type switch
            {
                ShaderInputType.Texture => GetTextureTypeName(resourceBinding),
                ShaderInputType.Sampler => resourceBinding.Flags.HasFlag(ShaderInputFlags.ComparisonSampler) ?
                                        "SamplerComparisonState" :
                                        "SamplerState",
                ShaderInputType.ByteAddress => "ByteAddressBuffer",
                ShaderInputType.Structured => "StructuredBuffer",
                ShaderInputType.UavRwByteAddress => "RWByteAddressBuffer",
                ShaderInputType.UavRwStructuredWithCounter or ShaderInputType.UavRwStructured => "RWStructuredBuffer",
                ShaderInputType.UavRwTyped => GetUAVTypeName(resourceBinding),
                ShaderInputType.UavAppendStructured => "AppendStructuredBuffer",
                ShaderInputType.UavConsumeStructured => "ConsumeStructuredBuffer",
                _ => throw new ArgumentException($"Unexpected resource binding type {resourceBinding.Type}"),
            };
        }

        void WriteResourceBinding(ResourceBinding resourceBinding)
        {
            Output.AppendLine(resourceBinding.ToString());
            if (resourceBinding.Type == ShaderInputType.CBuffer || resourceBinding.Type == ShaderInputType.TBuffer)
            {
                return;
            }
            if (resourceBinding.Type.IsUav())
            {
                var dcl = Container.Shader.DeclarationTokens
                    .OfType<UnorderedAccessViewDeclarationTokenBase>()
                    .Single(t => t.Operand.Indices[0].Value == resourceBinding.ID);
                if (dcl.Coherency == UnorderedAccessViewCoherency.GloballyCoherent)
                {
                    Output.Append("globallycoherent ");
                }
            }
            var type = GetBindingName(resourceBinding);
            Output.Append(type);
            if (resourceBinding.Type == ShaderInputType.Texture || resourceBinding.Type == ShaderInputType.UavRwTyped)
            {
                if (!(resourceBinding.ReturnType == ResourceReturnType.Float &&
                    resourceBinding.Flags.HasFlag(ShaderInputFlags.TextureComponents) &&
                    !resourceBinding.Dimension.IsMultiSampled()) || resourceBinding.Type == ShaderInputType.UavRwTyped)
                {
                    string returnType = GetReturnTypeDescription(resourceBinding.ReturnType);
                    if (resourceBinding.Flags.HasFlag(ShaderInputFlags.TextureComponent0) && !resourceBinding.Flags.HasFlag(ShaderInputFlags.TextureComponent1))
                        returnType += "2";
                    if (!resourceBinding.Flags.HasFlag(ShaderInputFlags.TextureComponent0) && resourceBinding.Flags.HasFlag(ShaderInputFlags.TextureComponent1))
                        returnType += "3";
                    if (resourceBinding.Flags.HasFlag(ShaderInputFlags.TextureComponent0) && resourceBinding.Flags.HasFlag(ShaderInputFlags.TextureComponent1))
                        returnType += "4";
                    if (resourceBinding.Dimension.IsMultiSampled() && resourceBinding.NumSamples > 0)
                    {
                        Output.AppendFormat("<{0}, {1}>", returnType, resourceBinding.NumSamples);
                    }
                    else
                    {
                        Output.AppendFormat("<{0}>", returnType);
                    }
                }
            }
            if (resourceBinding.Type.IsStructured())
            {
                var typeName = GetStructuredTypeName(resourceBinding);
                Output.AppendFormat("<{0}>", typeName);
            }
            Output.Append($" {resourceBinding.Name}");
            if (EmitRegisterDeclarations)
            {
                Output.Append($" : register({resourceBinding.GetBindPointDescription()})");
            }
            Output.AppendLine($";");
        }
        string GetStructuredTypeName(ResourceBinding resourceBinding)
        {
            var buffer = Container.ResourceDefinition.ConstantBuffers.Single(cb =>
                cb.BufferType == ConstantBufferType.ResourceBindInformation
                && cb.Name == resourceBinding.Name);
            var typeName = "";
            if (buffer.Variables[0].ShaderType.VariableClass == ShaderVariableClass.Struct)
            {
                typeName = buffer.Variables[0].ShaderType.BaseTypeName;
                if (string.IsNullOrEmpty(typeName))
                {
                    var index = Container.ResourceDefinition.ConstantBuffers
                        .Where(cb => cb.BufferType == ConstantBufferType.ResourceBindInformation)
                        .ToList()
                        .FindIndex(cb => cb == buffer);
                    typeName = $"struct{index}";
                }
                typeName = $"struct {typeName}";
            }
            else
            {
                typeName = GetShaderTypeName(buffer.Variables[0].ShaderType);
            }
            return typeName;
        }

        static string GetReturnTypeDescription(ResourceReturnType resourceReturnType)
        {
            return resourceReturnType switch
            {
                ResourceReturnType.SInt => "int",
                ResourceReturnType.UNorm => "unorm float",
                ResourceReturnType.SNorm => "snorm float",
                _ => resourceReturnType.GetDescription(),
            };
        }

        void WriteVariable(ShaderVariable variable, int indentLevel = 1, bool packOffset = true)
        {
            Output.Append($"{GetShaderTypeDeclaration(variable.ShaderType, indentLevel, root: false)} {variable.Name}");
            if (variable.ShaderType.ElementCount > 0)
            {
                Output.AppendFormat("[{0}]", variable.ShaderType.ElementCount);
            }
            if (variable.DefaultValue != null)
            {
                if (variable.DefaultValue.Count > 1)
                {
                    Output.AppendFormat(" = {0}({1})",
                        GetShaderTypeDeclaration(variable.ShaderType),
                        string.Join(", ", variable.DefaultValue));
                }
                else
                {
                    Output.AppendFormat(" = {0}", variable.DefaultValue[0]);
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
                Output.AppendFormat(" : packoffset(c{0}{1})", variable.StartOffset / 16, componentPacking);
            }
            Output.Append($"; // Offset {variable.StartOffset} Size {variable.Size} CBSize {variable.Member.GetCBVarSize(true)}");
            if (variable.Flags.HasFlag(ShaderVariableFlags.Used))
            {
                Output.Append($" [unused]");
            }
            Output.AppendLine();
        }

        internal static string GetShaderTypeName(ShaderType variable)
        {
            switch (variable.VariableClass)
            {
                case ShaderVariableClass.InterfacePointer:
                    {
                        if (!string.IsNullOrEmpty(variable.BaseTypeName)) // BaseTypeName is only populated in SM 5.0
                        {
                            return variable.BaseTypeName;
                        }
                        else
                        {
                            return string.Format("{0}{1}",
                                variable.VariableClass.GetDescription(),
                                variable.VariableType.GetDescription());
                        }
                    }
            }
            return variable.ToString().Replace("//", "").Trim(); // TODO: Cleanup
        }

        string GetShaderTypeDeclaration(ShaderType type, int indent = 0, string overrideName = null, bool root = true)
        {
            var sb = new StringBuilder();
            string indentString = new(' ', indent * 4);
            string baseTypeName = overrideName ?? type.BaseTypeName;
            switch (type.VariableClass)
            {
                case ShaderVariableClass.InterfacePointer:
                case ShaderVariableClass.MatrixColumns:
                case ShaderVariableClass.MatrixRows:
                    {
                        sb.Append(indentString);
                        if (!string.IsNullOrEmpty(type.BaseTypeName)) // BaseTypeName is only populated in SM 5.0
                        {
                            sb.Append(string.Format("{0}{1}", type.VariableClass.GetDescription(), baseTypeName));
                        }
                        else
                        {
                            sb.Append(type.VariableClass.GetDescription());
                            sb.Append(type.VariableType.GetDescription());
                            if (type.Columns > 1)
                            {
                                sb.Append(type.Columns);
                                if (type.Rows > 1)
                                    sb.Append("x" + type.Rows);
                            }
                        }
                        break;
                    }
                case ShaderVariableClass.Vector:
                    {
                        sb.Append(indentString + type.VariableType.GetDescription());
                        sb.Append(type.Columns);
                        break;
                    }
                case ShaderVariableClass.Struct:
                    {
                        //SM4 doesn't include typenames, check by signature
                        //TODO
                        if (root || baseTypeName == null || baseTypeName.EndsWith("<unnamed>"))
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
                        sb.Append(indentString + type.VariableType.GetDescription());
                        break;
                    }
                default:
                    throw new InvalidOperationException(string.Format("Variable class '{0}' is not currently supported.", type.VariableClass));
            }
            return sb.ToString();
        }
        string GetShaderMemberDeclaration(ShaderTypeMember member, int indent)
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
