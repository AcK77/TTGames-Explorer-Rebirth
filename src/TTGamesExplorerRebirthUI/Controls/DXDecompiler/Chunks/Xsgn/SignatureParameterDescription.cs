using DXDecompiler.Chunks.Common;
using DXDecompiler.Chunks.Shex;
using DXDecompiler.Util;

namespace DXDecompiler.Chunks.Xsgn
{
    /// <summary>
    /// Describes a shader signature.
    /// Based on D3D12_SIGNATURE_PARAMETER_DESC.
    /// </summary>
    public class SignatureParameterDescription
    {
        /// <summary>
        /// A per-parameter string that identifies how the data will be used.
        /// </summary>
        public string SemanticName { get; private set; }

        /// <summary>
        /// Semantic index that modifies the semantic. Used to differentiate different parameters that use the same 
        /// semantic.
        /// </summary>
        public uint SemanticIndex { get; private set; }

        /// <summary>
        /// The register that will contain this variable's data.
        /// </summary>
        public uint Register { get; private set; }

        /// <summary>
        /// A <see cref="SystemValueName"/>-typed value that identifies a predefined string that determines the 
        /// functionality of certain pipeline stages.
        /// </summary>
        public Name SystemValueType { get; private set; }

        /// <summary>
        /// A <see cref="RegisterComponentType"/>-typed value that identifies the per-component-data type that is 
        /// stored in a register. Each register can store up to four-components of data.
        /// </summary>
        public RegisterComponentType ComponentType { get; private set; }

        /// <summary>
        /// Mask which indicates which components of a register are used.
        /// </summary>
        public ComponentMask Mask { get; private set; }

        /// <summary>
        /// Mask which indicates whether a given component is never written (if the signature is an output signature) 
        /// or always read (if the signature is an input signature).
        /// </summary>
        public ComponentMask ReadWriteMask { get; private set; }

        /// <summary>
        /// Indicates which stream the geometry shader is using for the signature parameter.
        /// </summary>
        public uint Stream { get; private set; }

        ///// <summary>
        ///// A <see cref="MinPrecision"/>-typed value that indicates the minimum desired interpolation precision.
        ///// </summary>
        public MinPrecision MinPrecision { get; private set; }

        /// <summary>
        /// Gets the total number of bytes used by this parameter.
        /// </summary>
        public int ByteCount
        {
            get
            {
                var result = 0;
                if(Mask.HasFlag(ComponentMask.X))
                    result++;
                if(Mask.HasFlag(ComponentMask.Y))
                    result++;
                if(Mask.HasFlag(ComponentMask.Z))
                    result++;
                if(Mask.HasFlag(ComponentMask.W))
                    result++;
                return result * sizeof(float);
            }
        }

        public SignatureParameterDescription(string semanticName, uint semanticIndex,
            Name systemValueType, RegisterComponentType componentType, uint register,
            ComponentMask mask, ComponentMask readWriteMask)
        {
            SemanticName = semanticName;
            SemanticIndex = semanticIndex;
            Register = register;
            SystemValueType = systemValueType;
            ComponentType = componentType;
            Mask = mask;
            ReadWriteMask = readWriteMask;
        }

        public SignatureParameterDescription()
        {

        }

        public static SignatureParameterDescription Parse(BytecodeReader reader, BytecodeReader parameterReader,
            ChunkType chunkType, SignatureElementSize size)
        {
            uint stream = 0;
            if(size == SignatureElementSize._7 || size == SignatureElementSize._8)
                stream = parameterReader.ReadUInt32();
            uint nameOffset = parameterReader.ReadUInt32();
            var nameReader = reader.CopyAtOffset((int)nameOffset);

            var result = new SignatureParameterDescription
            {
                SemanticName = nameReader.ReadString(),
                SemanticIndex = parameterReader.ReadUInt32(),
                SystemValueType = (Name)parameterReader.ReadUInt32(),
                ComponentType = (RegisterComponentType)parameterReader.ReadUInt32(),
                Register = parameterReader.ReadUInt32(),
                Stream = stream,
            };

            uint mask = parameterReader.ReadUInt32();
            result.Mask = mask.DecodeValue<ComponentMask>(0, 7);
            result.ReadWriteMask = mask.DecodeValue<ComponentMask>(8, 15);

            if(size == SignatureElementSize._8)
            {
                MinPrecision minPrecision = (MinPrecision)parameterReader.ReadUInt32();
                result.MinPrecision = minPrecision;
            }

            // This is my guesswork, but it works so far...
            if(chunkType == ChunkType.Osg5 ||
                chunkType == ChunkType.Osgn ||
                chunkType == ChunkType.Osg1)
            {
                result.ReadWriteMask = (ComponentMask)(ComponentMask.All - result.ReadWriteMask);
            }

            return result;
        }
        internal void UpdateVersion(ChunkType chunkType, ShaderVersion version)
        {
            ProgramType programType = version.ProgramType;

            if(chunkType == ChunkType.Pcsg && programType == ProgramType.HullShader)
            {
                ReadWriteMask = (ComponentMask)(ComponentMask.All - ReadWriteMask);
            }
            // Vertex and pixel shaders need special handling for SystemValueType in the output signature.
            if((programType == ProgramType.PixelShader || programType == ProgramType.VertexShader)
                && (chunkType == ChunkType.Osg5 || chunkType == ChunkType.Osgn || chunkType == ChunkType.Osg1))
            {
                if(Register == 0xffffffff)
                    switch(SemanticName.ToUpper())
                    {
                        case "SV_DEPTH":
                            SystemValueType = Name.Depth;
                            break;
                        case "SV_COVERAGE":
                            SystemValueType = Name.Coverage;
                            break;
                        case "SV_DEPTHGREATEREQUAL":
                            SystemValueType = Name.DepthGreaterEqual;
                            break;
                        case "SV_DEPTHLESSEQUAL":
                            SystemValueType = Name.DepthLessEqual;
                            break;
                        case "SV_STENCILREF":
                            SystemValueType = Name.StencilRef;
                            break;
                    }
                else if(programType == ProgramType.PixelShader)
                {
                    SystemValueType = Name.Target;
                }
            }
        }
        public string GetTypeFormat()
        {
            if(MinPrecision == MinPrecision.None)
            {
                return ComponentType.GetDescription();
            }
            else
            {
                return MinPrecision.GetDescription();
            }
        }

        string GetSVRegisterName()
        {
            switch(SemanticName.ToUpper())
            {
                case "SV_DEPTH":
                    return "oDepth";
                case "SV_DEPTHGREATEREQUAL":
                    return "oDepthGE";
                case "SV_DEPTHLESSEQUAL":
                    return "oDepthLE";
                case "SV_COVERAGE":
                    return "oMask";
                case "SV_STENCILREF":
                    return "oStencilRef";
            }
            if(SystemValueType == Name.PrimitiveID)
            {
                return "primID";
            }
            else
            {
                return "special";
            }
        }

        public string ToString(bool includeStreams)
        {
            // For example:
            // Name                 Index   Mask Register SysValue  Format   Used
            // TEXCOORD                 0   xyzw        0     NONE   float   xyzw
            // SV_DepthGreaterEqual     0    N/A oDepthGE  DEPTHGE   float    YES
            string name = SemanticName;
            if(includeStreams)
            {
                name = string.Format("m{0}:{1}", Stream, SemanticName);
            }
            if(Register != uint.MaxValue)
            {
                return string.Format("{0,-20} {1,5}   {2,-4} {3,8} {4,8} {5,7}   {6,-4}", 
                    name, SemanticIndex,
                    Mask.GetDescription(),
                    Register, SystemValueType.GetDescription(),
                    GetTypeFormat(), ReadWriteMask.GetDescription());
            }
            else
            {
                var registerName = GetSVRegisterName();
                if(registerName.Length > 8)
                {
                    registerName = "    " + registerName;
                }
                return string.Format("{0,-20} {1,5}   {2,4}{3,9} {4,8} {5,7}   {6,4}",
                    name, SemanticIndex,
                    "N/A", registerName, SystemValueType.GetDescription(),
                    GetTypeFormat(), ReadWriteMask == 0 ? "NO" : "YES");
            }
        }
        public override string ToString()
        {
            return ToString(false);
        }
    }
}