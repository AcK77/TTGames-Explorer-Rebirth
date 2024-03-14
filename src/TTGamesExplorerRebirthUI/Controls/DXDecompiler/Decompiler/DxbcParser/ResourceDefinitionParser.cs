using DXDecompiler.Chunks.Common;
using DXDecompiler.Chunks.Rdef;
using DXDecompiler.Decompiler.IR;
using DXDecompiler.Decompiler.IR.ResourceDefinitions;
using System.Text;

namespace DXDecompiler.Decompiler.DxbcParser
{
    public class ResourceDefinitionParser
    {
        public static void Parse(IrShader shader, ResourceDefinitionChunk chunk)
        {
            var rdef = new IrResourceDefinition(chunk);
            shader.ResourceDefinition = rdef;
            rdef.ResourceBindingsDebug = DisassembleBindings(chunk.ResourceBindings, chunk.Target.IsSM51);
            rdef.ResourceBindings = [];
            foreach (var binding in chunk.ResourceBindings)
            {
                rdef.ResourceBindings.Add(ParseBinding(binding));
            }
            rdef.ConstantBuffersDebug = DisassembleBuffers(chunk.ConstantBuffers);
            rdef.ConstantBuffers = [];
            foreach (var cbuffer in chunk.ConstantBuffers)
            {
                rdef.ConstantBuffers.Add(ParseBuffer(cbuffer));
            }
        }

        static IrResourceBinding ParseBinding(ResourceBinding binding)
        {
            var irBinding = new IrResourceBinding()
            {
                Name = binding.Name,
                Class = GetResourceClass(binding),
                Kind = GetResourceKind(binding),
                Dimension = binding.NumSamples == uint.MaxValue ? 0 : binding.NumSamples,
                BindPoint = binding.BindPoint,
                BindCount = binding.BindCount,
                Debug = binding.ToString()
            };
            if ((irBinding.Class == IrResourceClass.SRV || irBinding.Class == IrResourceClass.UAV)
                && irBinding.Kind != IrResourceKind.TBuffer)
            {
                irBinding.ReturnType = ParseShaderReturnType(binding);
            }
            return irBinding;
        }

        static IrShaderType ParseShaderReturnType(ResourceBinding binding)
        {
            var returnType = new IrShaderType();
            switch (binding.ReturnType)
            {
                case ResourceReturnType.Float:
                    returnType.VariableType = IrShaderVariableType.F32;
                    break;
                case ResourceReturnType.Double:
                    returnType.VariableType = IrShaderVariableType.F64;
                    break;
                case ResourceReturnType.SInt:
                    returnType.VariableType = IrShaderVariableType.I32;
                    break;
                case ResourceReturnType.UInt:
                    returnType.VariableType = IrShaderVariableType.U32;
                    break;
                case ResourceReturnType.UNorm:
                    returnType.VariableType = IrShaderVariableType.UNormF32;
                    break;
                case ResourceReturnType.SNorm:
                    returnType.VariableType = IrShaderVariableType.SNormF32;
                    break;
                case ResourceReturnType.Mixed:
                    switch (binding.Type)
                    {
                        case ShaderInputType.Structured:
                        case ShaderInputType.UavRwStructured:
                        case ShaderInputType.UavAppendStructured:
                        case ShaderInputType.UavConsumeStructured:
                        case ShaderInputType.UavRwStructuredWithCounter:
                            break;
                        case ShaderInputType.ByteAddress:
                        case ShaderInputType.UavRwByteAddress:
                            //TODO: Byte type?
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            switch (binding.Type)
            {
                case ShaderInputType.Structured:
                case ShaderInputType.UavRwStructured:
                case ShaderInputType.UavAppendStructured:
                case ShaderInputType.UavConsumeStructured:
                case ShaderInputType.UavRwStructuredWithCounter:
                    returnType.VariableClass = ShaderVariableClass.Struct;
                    returnType.BaseTypeName = "TODO";
                    break;
                default:
                    ushort componentCount = 1;
                    if (binding.Flags.HasFlag(ShaderInputFlags.TextureComponent0))
                    {
                        componentCount += 1;
                    }
                    if (binding.Flags.HasFlag(ShaderInputFlags.TextureComponent1))
                    {
                        componentCount += 2;
                    }
                    returnType.Columns = componentCount;
                    returnType.VariableClass = componentCount == 1 ?
                        ShaderVariableClass.Scalar :
                        ShaderVariableClass.Vector;
                    break;
            }
            return returnType;
        }

        static IrConstantBuffer ParseBuffer(ConstantBuffer buffer)
        {
            return new IrConstantBuffer()
            {
                Name = buffer.Name,
                BufferType = buffer.BufferType,
                Variables = buffer.Variables
                    .Select(v => ParseVariable(v))
                    .ToList()
            };
        }

        static IrShaderVariable ParseVariable(ShaderVariable variable)
        {
            return new IrShaderVariable()
            {
                Member = ParseShaderTypeMember(variable.Member),
                BaseType = variable.BaseType,
                Size = variable.Size,
                Flags = variable.Flags,
                DefaultValue = variable.DefaultValue,
                StartTexture = variable.StartTexture,
                TextureSize = variable.TextureSize,
                StartSampler = variable.StartSampler,
                SamplerSize = variable.SamplerSize
            };
        }

        static IrShaderTypeMember ParseShaderTypeMember(ShaderTypeMember member)
        {
            return new IrShaderTypeMember()
            {
                Name = member.Name,
                Offset = member.Offset,
                Type = ParseShaderType(member.Type)
            };
        }

        static IrShaderType ParseShaderType(ShaderType type)
        {
            if (type == null) return null;
            return new IrShaderType()
            {
                VariableClass = type.VariableClass,
                VariableType = GetShaderVariableType(type.VariableType),
                Rows = type.Rows,
                Columns = type.Columns,
                ElementCount = type.ElementCount,
                Members = type.Members
                    .Select(m => ParseShaderTypeMember(m))
                    .ToList(),
                SubType = ParseShaderType(type.SubType),
                BaseClass = ParseShaderType(type.BaseClass),
                Interfaces = type.Interfaces
                    .Select(t => ParseShaderType(t))
                    .ToList(),
            };
        }

        static IrShaderVariableType GetShaderVariableType(ShaderVariableType type)
        {
            return type switch
            {
                ShaderVariableType.Void => IrShaderVariableType.Void,
                ShaderVariableType.Bool => IrShaderVariableType.I1,
                ShaderVariableType.Int => IrShaderVariableType.I32,
                ShaderVariableType.Float => IrShaderVariableType.F32,
                ShaderVariableType.Double => IrShaderVariableType.F64,
                ShaderVariableType.UInt => IrShaderVariableType.U32,
                ShaderVariableType.InterfacePointer => IrShaderVariableType.InterfacePointer,
                _ => throw new ArgumentException($"Unexpected shader variable type {type}"),
            };
        }

        public static string DisassembleBindings(List<ResourceBinding> bindings, bool IsSM51)
        {
            var sb = new StringBuilder();
            if (bindings.Count != 0)
            {
                if (!IsSM51)
                {
                    sb.AppendLine("// Name                                 Type  Format         Dim      HLSL Bind  Count");
                    sb.AppendLine("// ------------------------------ ---------- ------- ----------- -------------- ------");
                }
                else
                {
                    sb.AppendLine("// Name                                 Type  Format         Dim      ID      HLSL Bind  Count");
                    sb.AppendLine("// ------------------------------ ---------- ------- ----------- ------- -------------- ------");

                }
                foreach (var resourceBinding in bindings)
                    sb.AppendLine(resourceBinding.ToString());
            }
            return sb.ToString();
        }

        public static string DisassembleBuffers(List<ConstantBuffer> buffers)
        {
            return string.Join("\n", buffers.Select(b => b.ToString()));
        }

        static IrResourceClass GetResourceClass(ResourceBinding binding)
        {
            return binding.Type switch
            {
                ShaderInputType.CBuffer => IrResourceClass.CBuffer,
                ShaderInputType.Sampler => IrResourceClass.Sampler,
                ShaderInputType.TBuffer or ShaderInputType.Texture or ShaderInputType.Structured or ShaderInputType.ByteAddress => IrResourceClass.SRV,
                ShaderInputType.UavRwTyped or ShaderInputType.UavRwStructured or ShaderInputType.UavRwByteAddress or ShaderInputType.UavAppendStructured or 
                ShaderInputType.UavConsumeStructured or ShaderInputType.UavRwStructuredWithCounter => IrResourceClass.UAV,
                _ => throw new ArgumentException($"Unexpected Binding Type {binding.Type}"),
            };
        }

        static IrResourceKind GetResourceKind(ResourceBinding binding)
        {
            return binding.Type switch
            {
                ShaderInputType.CBuffer => IrResourceKind.CBuffer,
                ShaderInputType.TBuffer => IrResourceKind.TBuffer,
                ShaderInputType.Texture or ShaderInputType.UavRwTyped => binding.Dimension switch
                {
                    ShaderResourceViewDimension.Texture1D => IrResourceKind.Texture1D,
                    ShaderResourceViewDimension.Texture1DArray => IrResourceKind.Texture1DArray,
                    ShaderResourceViewDimension.Texture2D => IrResourceKind.Texture2D,
                    ShaderResourceViewDimension.Texture2DArray => IrResourceKind.Texture2DArray,
                    ShaderResourceViewDimension.Texture2DMultiSampled => IrResourceKind.Texture2DMS,
                    ShaderResourceViewDimension.Texture2DMultiSampledArray => IrResourceKind.Texture2DMSArray,
                    ShaderResourceViewDimension.Texture3D => IrResourceKind.Texture3D,
                    ShaderResourceViewDimension.TextureCube => IrResourceKind.TextureCube,
                    ShaderResourceViewDimension.TextureCubeArray => IrResourceKind.TextureCubeArray,
                    ShaderResourceViewDimension.Buffer => IrResourceKind.TypedBuffer,
                    _ => throw new ArgumentException($"Unexpected Binding Dimension: {binding.Dimension}"),
                },
                ShaderInputType.ByteAddress or ShaderInputType.UavRwByteAddress => IrResourceKind.RawBuffer,
                ShaderInputType.Structured or ShaderInputType.UavRwStructured or ShaderInputType.UavAppendStructured or ShaderInputType.UavConsumeStructured or ShaderInputType.UavRwStructuredWithCounter => IrResourceKind.StructuredBuffer,
                ShaderInputType.Sampler => IrResourceKind.Sampler,
                _ => throw new ArgumentException($"Unexpected Binding Type: {binding.Type}"),
            };
        }
    }
}