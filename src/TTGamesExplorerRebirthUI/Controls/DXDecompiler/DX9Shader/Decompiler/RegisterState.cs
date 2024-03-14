using DXDecompiler.DX9Shader.Bytecode.Ctab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DXDecompiler.DX9Shader
{
    public sealed class RegisterState
    {
        public readonly bool ColumnMajorOrder = true;

        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
        private readonly ICollection<ConstantInt> constantIntDefinitions = new List<ConstantInt>();
        private readonly ICollection<Constant> constantDefinitions = new List<Constant>();

        private ICollection<Constant> ConstantDefinitions => constantDefinitions; private ICollection<ConstantInt> ConstantIntDefinitions => constantIntDefinitions; public IDictionary<RegisterKey, RegisterDeclaration> RegisterDeclarations = new Dictionary<RegisterKey, RegisterDeclaration>();

        public RegisterState(ShaderModel shader)
        {
            Load(shader);
        }

        public ICollection<ConstantDeclaration> ConstantDeclarations { get; private set; }

        public IDictionary<RegisterKey, RegisterDeclaration> MethodInputRegisters { get; } = new Dictionary<RegisterKey, RegisterDeclaration>();
        public IDictionary<RegisterKey, RegisterDeclaration> MethodOutputRegisters { get; } = new Dictionary<RegisterKey, RegisterDeclaration>();

        private void Load(ShaderModel shader)
        {
            ConstantDeclarations = shader.ConstantTable.ConstantDeclarations;
            foreach (var constantDeclaration in ConstantDeclarations)
            {
                var registerType = constantDeclaration.RegisterSet switch
                {
                    RegisterSet.Bool => RegisterType.ConstBool,
                    RegisterSet.Float4 => RegisterType.Const,
                    RegisterSet.Int4 => RegisterType.ConstInt,
                    RegisterSet.Sampler => RegisterType.Sampler,
                    _ => throw new InvalidOperationException(),
                };
                if (registerType == RegisterType.Sampler)
                {
                    // Use declaration from declaration instruction instead
                    continue;
                }
                for (uint r = 0; r < constantDeclaration.RegisterCount; r++)
                {
                    var registerKey = new RegisterKey(registerType, constantDeclaration.RegisterIndex + r);
                    var registerDeclaration = new RegisterDeclaration(registerKey);
                    RegisterDeclarations.Add(registerKey, registerDeclaration);
                }
            }

            foreach (var instruction in shader.Tokens.OfType<InstructionToken>().Where(i => i.HasDestination))
            {
                if (instruction.Opcode == Opcode.Dcl)
                {
                    var registerDeclaration = new RegisterDeclaration(instruction);
                    RegisterKey registerKey = registerDeclaration.RegisterKey;

                    RegisterDeclarations.Add(registerKey, registerDeclaration);

                    switch (registerKey.Type)
                    {
                        case RegisterType.Input:
                        case RegisterType.MiscType:
                        case RegisterType.Texture when shader.Type == ShaderType.Pixel:
                            MethodInputRegisters.Add(registerKey, registerDeclaration);
                            break;
                        case RegisterType.Output:
                        case RegisterType.ColorOut:
                        case RegisterType.AttrOut when shader.MajorVersion == 3 && shader.Type == ShaderType.Vertex:
                            MethodOutputRegisters.Add(registerKey, registerDeclaration);
                            break;
                        case RegisterType.Sampler:
                        case RegisterType.Addr:
                            break;
                        default:
                            throw new Exception($"Unexpected dcl {registerKey.Type}");
                    }
                }
                else if (instruction.Opcode == Opcode.Def)
                {
                    var constant = new Constant(
                        instruction.GetParamRegisterNumber(0),
                        instruction.GetParamSingle(1),
                        instruction.GetParamSingle(2),
                        instruction.GetParamSingle(3),
                        instruction.GetParamSingle(4));
                    ConstantDefinitions.Add(constant);
                }
                else if (instruction.Opcode == Opcode.DefI)
                {
                    var constantInt = new ConstantInt(instruction.GetParamRegisterNumber(0),
                        instruction.Data[1],
                        instruction.Data[2],
                        instruction.Data[3],
                        instruction.Data[4]);
                    ConstantIntDefinitions.Add(constantInt);
                }
                else
                {
                    // Find all assignments to color outputs, because pixel shader outputs are not declared.
                    int destIndex = instruction.GetDestinationParamIndex();
                    RegisterType registerType = instruction.GetParamRegisterType(destIndex);
                    var registerNumber = instruction.GetParamRegisterNumber(destIndex);
                    var registerKey = new RegisterKey(registerType, registerNumber);
                    if (RegisterDeclarations.ContainsKey(registerKey) == false)
                    {
                        var reg = new RegisterDeclaration(registerKey);
                        RegisterDeclarations[registerKey] = reg;
                        switch (registerType)
                        {
                            case RegisterType.AttrOut:
                            case RegisterType.ColorOut:
                            case RegisterType.DepthOut:
                            case RegisterType.Output:
                            case RegisterType.RastOut:
                                MethodOutputRegisters[registerKey] = reg;
                                break;

                        }
                    }

                }
            }
        }

        public string GetDestinationName(InstructionToken instruction)
        {
            int destIndex = instruction.GetDestinationParamIndex();
            RegisterKey registerKey = instruction.GetParamRegisterKey(destIndex);

            string registerName = GetRegisterName(registerKey);
            registerName ??= instruction.GetParamRegisterName(destIndex);
            var registerLength = GetRegisterFullLength(registerKey);
            string writeMaskName = instruction.GetDestinationWriteMaskName(registerLength, true);

            return string.Format("{0}{1}", registerName, writeMaskName);
        }

        public string GetSourceName(InstructionToken instruction, int srcIndex)
        {
            string sourceRegisterName;
            RegisterKey registerKey = instruction.GetParamRegisterKey(srcIndex);
            var registerType = instruction.GetParamRegisterType(srcIndex);
            switch (registerType)
            {
                case RegisterType.Const:
                case RegisterType.Const2:
                case RegisterType.Const3:
                case RegisterType.Const4:
                case RegisterType.ConstBool:
                case RegisterType.ConstInt:
                    sourceRegisterName = GetSourceConstantName(instruction, srcIndex);
                    if (sourceRegisterName != null)
                    {
                        return sourceRegisterName;
                    }
                    switch (registerType)
                    {
                        case RegisterType.Const:
                        case RegisterType.Const2:
                        case RegisterType.Const3:
                        case RegisterType.Const4:
                            break;
                        case RegisterType.ConstBool:
                            break;
                        case RegisterType.ConstInt:
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    var registerNumber = instruction.GetParamRegisterNumber(srcIndex);
                    ConstantDeclaration decl = FindConstant(registerNumber);
                    if (decl == null)
                    {
                        // Constant register not found in def statements nor the constant table
                        //TODO:
                        return $"Error {registerType}{registerNumber}";
                        //throw new NotImplementedException();
                    }
                    var totalOffset = registerNumber - decl.RegisterIndex;
                    var data = decl.GetRegisterTypeByOffset(totalOffset);
                    var offsetFromMember = registerNumber - data.RegisterIndex;
                    sourceRegisterName = decl.GetMemberNameByOffset(totalOffset);
                    if (data.Type.ParameterClass == ParameterClass.MatrixRows)
                    {
                        sourceRegisterName = string.Format("{0}[{1}]", decl.Name, offsetFromMember);
                    }
                    else if (data.Type.ParameterClass == ParameterClass.MatrixColumns)
                    {
                        sourceRegisterName = string.Format("transpose({0})[{1}]", decl.Name, offsetFromMember);
                    }
                    break;
                default:
                    sourceRegisterName = GetRegisterName(registerKey);
                    break;
            }

            sourceRegisterName ??= instruction.GetParamRegisterName(srcIndex);

            sourceRegisterName += instruction.GetSourceSwizzleName(srcIndex, true);
            return ApplyModifier(instruction.GetSourceModifier(srcIndex), sourceRegisterName);
        }

        public uint GetRegisterFullLength(RegisterKey registerKey)
        {
            if (registerKey.Type == RegisterType.Const)
            {
                var constant = FindConstant(registerKey.Number);
                var data = constant.GetRegisterTypeByOffset(registerKey.Number - constant.RegisterIndex);
                if (data.Type.ParameterType != ParameterType.Float)
                {
                    throw new NotImplementedException();
                }
                if (data.Type.ParameterClass == ParameterClass.MatrixColumns)
                {
                    return data.Type.Rows;
                }
                return data.Type.Columns;
            }

            RegisterDeclaration decl = RegisterDeclarations[registerKey];
            return decl.TypeName switch
            {
                "float" => 1,
                "float2" => 2,
                "float3" => 3,
                "float4" => 4,
                _ => throw new InvalidOperationException(),
            };
        }

        public string GetRegisterName(RegisterKey registerKey)
        {
            var decl = RegisterDeclarations[registerKey];
            switch (registerKey.Type)
            {
                case RegisterType.Texture:
                case RegisterType.Input:
                    return (MethodInputRegisters.Count == 1) ? decl.Name : ("i." + decl.Name);
                case RegisterType.RastOut:
                case RegisterType.Output:
                case RegisterType.AttrOut:
                case RegisterType.ColorOut:
                    return (MethodOutputRegisters.Count == 1) ? decl.Name : ("o." + decl.Name);
                case RegisterType.Const:
                    var constDecl = FindConstant(registerKey.Number);
                    var relativeOffset = registerKey.Number - constDecl.RegisterIndex;
                    var name = constDecl.GetMemberNameByOffset(relativeOffset);
                    var data = constDecl.GetRegisterTypeByOffset(relativeOffset);

                    // sanity check
                    var registersOccupied = data.Type.ParameterClass == ParameterClass.MatrixColumns
                        ? data.Type.Columns
                        : data.Type.Rows;
                    if (registersOccupied == 1 && data.RegisterIndex != registerKey.Number)
                    {
                        throw new InvalidOperationException();
                    }

                    switch (data.Type.ParameterType)
                    {
                        case ParameterType.Float:
                            if (registersOccupied == 1)
                            {
                                return name;
                            }
                            var subElement = (registerKey.Number - data.RegisterIndex).ToString();
                            return ColumnMajorOrder
                                ? $"transpose({name})[{subElement}]" // subElement = col
                                : $"{name}[{subElement}]"; // subElement = row;
                        default:
                            throw new NotImplementedException();
                    }
                case RegisterType.Sampler:
                    ConstantDeclaration samplerDecl = FindConstant(RegisterSet.Sampler, registerKey.Number);
                    if (samplerDecl != null)
                    {
                        var offset = registerKey.Number - samplerDecl.RegisterIndex;
                        return samplerDecl.GetMemberNameByOffset(offset);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                case RegisterType.MiscType:
                    return registerKey.Number switch
                    {
                        0 => "vFace",
                        1 => "vPos",
                        _ => throw new NotImplementedException(),
                    };
                case RegisterType.Temp:
                    return registerKey.ToString();
                default:
                    throw new NotImplementedException();
            }
        }

        public ConstantDeclaration FindConstant(RegisterInputNode register)
        {
            if (register.RegisterComponentKey.Type != RegisterType.Const)
            {
                return null;
            }
            return FindConstant(ParameterType.Float, register.RegisterComponentKey.Number);
        }

        public ConstantDeclaration FindConstant(RegisterSet set, uint index)
        {
            return ConstantDeclarations.FirstOrDefault(c =>
                c.RegisterSet == set &&
                c.ContainsIndex(index));
        }

        public ConstantDeclaration FindConstant(ParameterType type, uint index)
        {
            return ConstantDeclarations.FirstOrDefault(c =>
                c.ParameterType == type &&
                c.ContainsIndex(index));
        }


        public ConstantDeclaration FindConstant(uint index)
        {
            return ConstantDeclarations.FirstOrDefault(c => c.ContainsIndex(index));
        }

        private string GetSourceConstantName(InstructionToken instruction, int srcIndex)
        {
            var registerType = instruction.GetParamRegisterType(srcIndex);
            var registerNumber = instruction.GetParamRegisterNumber(srcIndex);

            switch (registerType)
            {
                case RegisterType.ConstBool:
                    //throw new NotImplementedException();
                    return null;
                case RegisterType.ConstInt:
                    {
                        var constantInt = ConstantIntDefinitions.FirstOrDefault(x => x.RegisterIndex == registerNumber);
                        if (constantInt == null)
                        {
                            return null;
                        }
                        byte[] swizzle = instruction.GetSourceSwizzleComponents(srcIndex);
                        uint[] constant = [
                                constantInt[swizzle[0]],
                                constantInt[swizzle[1]],
                                constantInt[swizzle[2]],
                                constantInt[swizzle[3]] ];

                        switch (instruction.GetSourceModifier(srcIndex))
                        {
                            case SourceModifier.None:
                                break;
                            case SourceModifier.Negate:
                                throw new NotImplementedException();
                            /*
                            for (int i = 0; i < 4; i++)
                            {
                                constant[i] = -constant[i];
                            }
                            break;*/
                            default:
                                throw new NotImplementedException();
                        }

                        int destLength = instruction.GetDestinationMaskLength();
                        switch (destLength)
                        {
                            case 1:
                                return constant[0].ToString();
                            case 2:
                                if (constant[0] == constant[1])
                                {
                                    return constant[0].ToString();
                                }
                                return $"int2({constant[0]}, {constant[1]})";
                            case 3:
                                if (constant[0] == constant[1] && constant[0] == constant[2])
                                {
                                    return constant[0].ToString();
                                }
                                return $"int3({constant[0]}, {constant[1]}, {constant[2]})";
                            case 4:
                                if (constant[0] == constant[1] && constant[0] == constant[2] && constant[0] == constant[3])
                                {
                                    return constant[0].ToString();
                                }
                                return $"int4({constant[0]}, {constant[1]}, {constant[2]}, {constant[3]})";
                            default:
                                throw new InvalidOperationException();
                        }
                    }

                case RegisterType.Const:
                case RegisterType.Const2:
                case RegisterType.Const3:
                case RegisterType.Const4:
                    {
                        var constantRegister = ConstantDefinitions.FirstOrDefault(x => x.RegisterIndex == registerNumber);
                        if (constantRegister == null)
                        {
                            return null;
                        }

                        byte[] swizzle = instruction.GetSourceSwizzleComponents(srcIndex);
                        float[] constant = [
                            constantRegister[swizzle[0]],
                            constantRegister[swizzle[1]],
                            constantRegister[swizzle[2]],
                            constantRegister[swizzle[3]] ];

                        switch (instruction.GetSourceModifier(srcIndex))
                        {
                            case SourceModifier.None:
                                break;
                            case SourceModifier.Negate:
                                for (int i = 0; i < 4; i++)
                                {
                                    constant[i] = -constant[i];
                                }
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        int destLength;
                        if (instruction.HasDestination)
                        {
                            destLength = instruction.GetDestinationMaskLength();
                        }
                        else
                        {
                            if (instruction.Opcode == Opcode.If || instruction.Opcode == Opcode.IfC)
                            {
                                // TODO
                            }
                            destLength = 4;
                        }
                        switch (destLength)
                        {
                            case 1:
                                return constant[0].ToString(_culture);
                            case 2:
                                if (constant[0] == constant[1])
                                {
                                    return constant[0].ToString(_culture);
                                }
                                return string.Format("float2({0}, {1})",
                                    constant[0].ToString(_culture),
                                    constant[1].ToString(_culture));
                            case 3:
                                if (constant[0] == constant[1] && constant[0] == constant[2])
                                {
                                    return constant[0].ToString(_culture);
                                }
                                return string.Format("float3({0}, {1}, {2})",
                                    constant[0].ToString(_culture),
                                    constant[1].ToString(_culture),
                                    constant[2].ToString(_culture));
                            case 4:
                                if (constant[0] == constant[1] && constant[0] == constant[2] && constant[0] == constant[3])
                                {
                                    return constant[0].ToString(_culture);
                                }
                                return string.Format("float4({0}, {1}, {2}, {3})",
                                    constant[0].ToString(_culture),
                                    constant[1].ToString(_culture),
                                    constant[2].ToString(_culture),
                                    constant[3].ToString(_culture));
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                default:
                    return null;
            }
        }


        private static string ApplyModifier(SourceModifier modifier, string value)
        {
            return modifier switch
            {
                SourceModifier.None => value,
                SourceModifier.Negate => $"-{value}",
                SourceModifier.Bias => $"{value}_bias",
                SourceModifier.BiasAndNegate => $"-{value}_bias",
                SourceModifier.Sign => $"{value}_bx2",
                SourceModifier.SignAndNegate => $"-{value}_bx2",
                SourceModifier.Complement => throw new NotImplementedException(),
                SourceModifier.X2 => $"(2 * {value})",
                SourceModifier.X2AndNegate => $"(-2 * {value})",
                SourceModifier.DivideByZ => $"{value}_dz",
                SourceModifier.DivideByW => $"{value}_dw",
                SourceModifier.Abs => $"abs({value})",
                SourceModifier.AbsAndNegate => $"-abs({value})",
                SourceModifier.Not => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}