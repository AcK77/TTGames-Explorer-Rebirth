using DXDecompiler.Chunks.Common;
using DXDecompiler.Util;
using System.Text;

namespace DXDecompiler.Chunks.Rdef
{
    public class ResourceDefinitionChunk : BytecodeChunk
    {
        public List<ConstantBuffer> ConstantBuffers { get; private set; }
        public List<ResourceBinding> ResourceBindings { get; private set; }
        public ShaderVersion Target { get; private set; }
        public ShaderFlags Flags { get; private set; }
        public string Creator { get; private set; }
        public uint InterfaceSlotCount { get; private set; }
        public ResourceDefinitionChunk()
        {
            ConstantBuffers = [];
            ResourceBindings = [];
        }

        public static ResourceDefinitionChunk Parse(BytecodeReader reader)
        {
            var headerReader = reader.CopyAtCurrentPosition();

            uint constantBufferCount = headerReader.ReadUInt32();
            uint constantBufferOffset = headerReader.ReadUInt32();
            uint resourceBindingCount = headerReader.ReadUInt32();
            uint resourceBindingOffset = headerReader.ReadUInt32();
            var target = ShaderVersion.ParseRdef(headerReader);
            uint flags = headerReader.ReadUInt32();

            var creatorOffset = headerReader.ReadUInt32();
            var creatorReader = reader.CopyAtOffset((int)creatorOffset);
            var creator = creatorReader.ReadString();

            var result = new ResourceDefinitionChunk
            {
                Target = target,
                Flags = (ShaderFlags)flags,
                Creator = creator
            };

#pragma warning disable IDE0059
            if (target.MajorVersion == 5 || target.ProgramType == ProgramType.LibraryShader)
            {
                var isVersion5_1 = target.MajorVersion == 5 && target.MinorVersion == 1;
                if (isVersion5_1)
                {
                    var unknown0 = headerReader.ReadUInt32();
                    // Debug.Assert(unknown0 == 625218323, $"Unexpected RDef Header identifier {unknown0}");
                }
                else
                {
                    string rd11 = headerReader.ReadUInt32().ToFourCcString();
                    if (rd11 != "RD11")
                        throw new ParseException("Expected RD11.");
                }

                var unknown1 = headerReader.ReadUInt32(); // TODO
                // Debug.Assert(unknown1 == 60);

                var constantBufferStride = headerReader.ReadUInt32();
                // Debug.Assert(constantBufferStride == 24);

                var resourceBindingStride = headerReader.ReadUInt32();
                // Debug.Assert(resourceBindingStride == (isVersion5_1 ? 40 : 32), $"Unexpected resource binding stride {resourceBindingStride}");

                //Shader Variable Stride?
                var unknown4 = headerReader.ReadUInt32();
                // Debug.Assert(unknown4 == 40);

                var unknown5 = headerReader.ReadUInt32();
                // Debug.Assert(unknown5 == 36);

                //Shader Type Member Stride?
                var unknown6 = headerReader.ReadUInt32();
                // Debug.Assert(unknown6 == 12);
#pragma warning restore IDE0059
                result.InterfaceSlotCount = headerReader.ReadUInt32();
            }

            var constantBufferReader = reader.CopyAtOffset((int)constantBufferOffset);
            for (int i = 0; i < constantBufferCount; i++)
                result.ConstantBuffers.Add(ConstantBuffer.Parse(reader, constantBufferReader, result.Target));

            var resourceBindingReader = reader.CopyAtOffset((int)resourceBindingOffset);
            for (int i = 0; i < resourceBindingCount; i++)
                result.ResourceBindings.Add(ResourceBinding.Parse(reader, resourceBindingReader, target));

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ConstantBuffers.Count != 0)
            {
                sb.AppendLine("// Buffer Definitions: ");
                sb.AppendLine("//");

                foreach (var constantBuffer in ConstantBuffers)
                    sb.Append(constantBuffer);

                sb.AppendLine("//");
            }

            if (ResourceBindings.Count != 0)
            {
                sb.AppendLine("// Resource Bindings:");
                sb.AppendLine("//");
                if (!Target.IsSM51)
                {
                    sb.AppendLine("// Name                                 Type  Format         Dim      HLSL Bind  Count");
                    sb.AppendLine("// ------------------------------ ---------- ------- ----------- -------------- ------");
                }
                else
                {
                    sb.AppendLine("// Name                                 Type  Format         Dim      ID      HLSL Bind  Count");
                    sb.AppendLine("// ------------------------------ ---------- ------- ----------- ------- -------------- ------");

                }
                foreach (var resourceBinding in ResourceBindings)
                    sb.AppendLine(resourceBinding.ToString());

                sb.AppendLine("//");
                sb.AppendLine("//");
            }

            return sb.ToString();
        }
    }
}