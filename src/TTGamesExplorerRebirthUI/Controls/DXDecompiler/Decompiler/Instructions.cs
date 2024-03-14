﻿using DXDecompiler.Chunks.Common;
using DXDecompiler.Chunks.Rdef;
using DXDecompiler.Chunks.Shex;
using DXDecompiler.Chunks.Shex.Tokens;
using System;
using System.Linq;

namespace DXDecompiler.Decompiler
{
	public partial class OldHLSLDecompiler
	{
		internal void TranslateInstruction(InstructionToken token)
		{
			DebugLog(token);
			switch(token.Header.OpcodeType)
			{
				case OpcodeType.Dtoi:
				case OpcodeType.Dtou:
				case OpcodeType.FtoI:
				case OpcodeType.FtoU:
					{
						var constructorType = (token.Header.OpcodeType == OpcodeType.FtoI ||
							token.Header.OpcodeType == OpcodeType.Dtoi) ?
							ShaderVariableType.Int :
							ShaderVariableType.UInt;
						var dest = token.Operands[0];
						var src = token.Operands[1];
						var destCount = dest.GetNumSwizzleElements();
						var srcCount = src.GetNumSwizzleElements();
						AddIndent();
						AddAssignToDest(token.Operands[0]);
						Output.Append(GetConstructorForType(constructorType,
							 srcCount == destCount ? destCount : 4));
						Output.Append("(");
						WriteOperand(src);
						Output.AppendLine(");");
						break;
					}
				case OpcodeType.DToF:
				case OpcodeType.ItoF:
				case OpcodeType.Utof:
					{
						var dest = token.Operands[0];
						var src = token.Operands[1];
						var destCount = dest.GetNumSwizzleElements();
						var srcCount = src.GetNumSwizzleElements();
						AddIndent();
						AddAssignToDest(token.Operands[0]);
						Output.Append(GetConstructorForType(ShaderVariableType.Float,
							 srcCount == destCount ? destCount : 4));
						Output.Append("(");
						WriteOperand(src);
						Output.AppendLine(");");
						break;
					}
				case OpcodeType.Itod:
				case OpcodeType.Utod:
				case OpcodeType.FToD:
					{
						var dest = token.Operands[0];
						var src = token.Operands[1];
						var destCount = dest.GetNumSwizzleElements();
						var srcCount = src.GetNumSwizzleElements();
						AddIndent();
						AddAssignToDest(token.Operands[0]);
						Output.Append(GetConstructorForType(ShaderVariableType.Double,
							 srcCount == destCount ? destCount : 4));
						Output.Append("(");
						WriteOperand(src);
						Output.AppendLine(");");
						break;
					}
				case OpcodeType.DMov:
				case OpcodeType.Mov:
					{
						WriteMoveBinaryOp(token.Operands[0], token.Operands[1]);
						break;
					}
				case OpcodeType.DAdd:
				case OpcodeType.IAdd:
				case OpcodeType.Add:
					{
						CallBinaryOp("+", token, 0, 1, 2);
						break;
					}
				case OpcodeType.Dfma:
				case OpcodeType.IMad:
				case OpcodeType.Mad:
					{
						CallTernaryOp("*", "+", token, 0, 1, 2, 3);
						break;
					}
				case OpcodeType.DMul:
				case OpcodeType.IMul:
				case OpcodeType.Mul:
					{
						CallBinaryOp("*", token, 0, 1, 2);
						break;
					}
				case OpcodeType.UDiv:
				case OpcodeType.Ddiv:
				case OpcodeType.Div:
					{
						CallBinaryOp("/", token, 0, 1, 2);
						break;
					}
				// Bitwise operations
				case OpcodeType.Or:
					{
						CallBinaryOp("|", token, 0, 1, 2);
						break;
					}
				case OpcodeType.And:
					{
						CallBinaryOp("&", token, 0, 1, 2);
						break;
					}
				case OpcodeType.FirstBitHi:
					{
						CallHelper("firstbithigh", token, 0, 1);
						break;
					}
				case OpcodeType.FirstBitSHi:
					{
						CallHelper("firstbithigh", token, 0, 1);
						break;
					}
				case OpcodeType.BfRev:
					{
						CallHelper("reversebits ", token, 0, 1);
						break;
					}
				case OpcodeType.Bfi:
					{
						string bfiFunc = @"int4 BFI(uint4 src0, uint4 src1, uint4 src2, uint4 src3){
	uint4 width = src0 & 0x1f;
	uint4 offset = src1 & 0x1f;
	uint4 mask = (((1 << width)-1) << offset) & 0xffffffff;
	int4 dest = ((src2 << offset) & bitmask) | (src3 & ~bitmask)
	return dest;
}";
						AddFunction("BFI", bfiFunc);
						CallHelper("BFI", token, 0, 1, 2, 3, 4);
						break;
					}
				case OpcodeType.UShr:
					{
						CallBinaryOp(">>", token, 0, 1, 2);
						break;
					}
				case OpcodeType.IShr:
					{
						CallBinaryOp(">>", token, 0, 1, 2);
						break;
					}
				case OpcodeType.IShl:
					{
						CallBinaryOp("<<", token, 0, 1, 2);
						break;
					}
				case OpcodeType.UBfe:
				case OpcodeType.IBfe:
					{
						string name = token.Header.OpcodeType == OpcodeType.UBfe ?
							"UBFE" : "IBFE";
						string returnType = token.Header.OpcodeType == OpcodeType.UBfe ?
							"uint4" : "int4";
						string template = @"{0} {1}(uint4 src1, uint4 src2, {0} value){{
	{0} result = 0;
	for(uint i = 0; i < 4; i++){{
		uint width = src1[i] & 0x1f;
		uint offset =src2[i] & 0x1f;
		if(width == 0){{
			result[i] = 0;
		}} else if(width + offset < 32){{
			result[i] = value << (32-(width + offset));
			result[i] = result[i] >> (32 - width);
		}} else {{
			result[i] = value >> (32 - width);
		}}
	}}
	return result;
}}";
						AddFunction(name, string.Format(template, returnType, name));
						CallHelper(name, token, 0, 1, 2, 3);
						break;
					}
				case OpcodeType.Not:
					{
						AddIndent();
						AddAssignToDest(token.Operands[0]);
						Output.Append("~");
						WriteOperand(token.Operands[1]);
						Output.AppendLine(";");
						break;
					}
				case OpcodeType.Xor:
					{
						CallBinaryOp("^", token, 0, 1, 2);
						break;
					}
				case OpcodeType.CountBits:
					{
						AddIndent();
						AddAssignToDest(token.Operands[0]);
						Output.Append(" = bitCount(");
						WriteOperand(token.Operands[1]);
						Output.AppendLine(");");
						break;
					}
				//Comparisons
				case OpcodeType.DNe:
				case OpcodeType.INe:
				case OpcodeType.Ne:
					{
						AddComparision(token, ComparisonType.Ne);
						break;
					}
				case OpcodeType.ILt:
				case OpcodeType.DLt:
				case OpcodeType.ULt:
				case OpcodeType.Lt:
					{
						AddComparision(token, ComparisonType.Lt);
						break;
					}
				case OpcodeType.IGe:
				case OpcodeType.UGe:
				case OpcodeType.DGe:
				case OpcodeType.Ge:
					{
						AddComparision(token, ComparisonType.Ge);
						break;
					}
				case OpcodeType.DEq:
				case OpcodeType.IEq:
				case OpcodeType.Eq:
					{
						AddComparision(token, ComparisonType.Eq);
						break;
					}
				case OpcodeType.DMovC:
				case OpcodeType.MovC:
					{
						AddMOVCBinaryOp(token.Operands[0], token.Operands[1],
							token.Operands[2], token.Operands[3]);
						break;
					}
				case OpcodeType.SwapC:
					{
						// TODO needs temps!!
						AddMOVCBinaryOp(token.Operands[0], token.Operands[2], token.Operands[4], token.Operands[3]);
						AddMOVCBinaryOp(token.Operands[1], token.Operands[2], token.Operands[3], token.Operands[4]);
						break;
					}
				//ControlFlow
				case OpcodeType.Loop:
					{
						AddIndent();
						Output.AppendLine("while(true){");
						indent++;
						break;
					}
				case OpcodeType.EndLoop:
					{
						indent--;
						AddIndent();
						Output.AppendLine("}");
						break;
					}
				case OpcodeType.Break:
					{
						AddIndent();
						Output.AppendLine("break;");
						break;
					}
				case OpcodeType.BreakC:
					{
						WriteConditional(token);
						break;
					}
				case OpcodeType.ContinueC:
					{
						WriteConditional(token);
						break;
					}
				case OpcodeType.If:
					{
						WriteConditional(token);
						break;
					}
				case OpcodeType.RetC:
					{
						WriteConditional(token);
						break;
					}
				case OpcodeType.Else:
					{
						indent--;
						AddIndent();
						Output.AppendLine("} else {");
						indent++;
						break;
					}
				case OpcodeType.EndSwitch:
				case OpcodeType.EndIf:
					{
						indent--;
						AddIndent();
						Output.AppendLine("}");
						break;
					}
				case OpcodeType.Continue:
					{
						Output.AppendLine("continue;");
						break;
					}
				case OpcodeType.Switch:
					{
						AddIndent();
						indent++;
						Output.Append("switch(int(");
						WriteOperand(token.Operands[0]);
						Output.AppendLine(")){");
						break;
					}
				case OpcodeType.Case:
					{
						AddIndent();
						indent++;
						Output.Append("case ");
						WriteOperand(token.Operands[0]);
						Output.AppendLine(":");
						break;
					}
				case OpcodeType.Default:
					{
						AddIndent();
						Output.AppendLine("default:");
						break;
					}
				case OpcodeType.Ret:
					{
						if(Container.Shader.Version.ProgramType != ProgramType.ComputeShader &&
							Container.Shader.Version.ProgramType != ProgramType.GeometryShader)
						{
							AddIndent();
							Output.AppendLine($"return output;");
						}
						break;
					}
				case OpcodeType.Label:
					{
						break;
					}
				//Functions
				case OpcodeType.Sincos:
					{
						if(token.Operands[0].OperandType == token.Operands[2].OperandType &&
							token.Operands[0].Indices[0].Value == token.Operands[2].Indices[0].Value)
						{
							if(token.Operands[1].OperandType != OperandType.Null)
							{
								CallHelper("cos", token, 1, 2);
							}
							if(token.Operands[0].OperandType != OperandType.Null)
							{
								CallHelper("cos", token, 0, 2);
							}
						}
						else
						{
							if(token.Operands[0].OperandType != OperandType.Null)
							{
								CallHelper("sin", token, 0, 2);
							}
							if(token.Operands[1].OperandType != OperandType.Null)
							{
								CallHelper("cos", token, 1, 2);
							}
						}
						break;
					}
				case OpcodeType.Dp2:
					{
						CallHelper("dot", token, 0, 1, 2);
						break;
					}
				case OpcodeType.Dp3:
					{
						CallHelper("dot", token, 0, 1, 2);
						break;
					}
				case OpcodeType.Dp4:
					{
						CallHelper("dot", token, 0, 1, 2);
						break;
					}
				case OpcodeType.Log:
					{
						CallHelper("log", token, 0, 1);
						break;
					}
				case OpcodeType.Rsq:
					{
						CallHelper("normalize", token, 0, 1);
						break;
					}
				case OpcodeType.Exp:
					{
						CallHelper("exp2", token, 0, 1);
						break;
					}
				case OpcodeType.Sqrt:
					{
						CallHelper("sqrt", token, 0, 1);
						break;
					}
				case OpcodeType.RoundPi:
					{
						CallHelper("ceil", token, 0, 1);
						break;
					}
				case OpcodeType.RoundNi:
					{
						CallHelper("floor", token, 0, 1);
						break;
					}
				case OpcodeType.RoundZ:
					{
						CallHelper("trunc", token, 0, 1);
						break;
					}
				case OpcodeType.RoundNe:
					{
						CallHelper("roundEven", token, 0, 1);
						break;
					}
				case OpcodeType.Frc:
					{
						CallHelper("frac", token, 0, 1);
						break;
					}
				case OpcodeType.IMax:
				case OpcodeType.UMax:
				case OpcodeType.DMax:
				case OpcodeType.Max:
					{
						CallHelper("max", token, 0, 1, 2);
						break;
					}
				case OpcodeType.IMin:
				case OpcodeType.UMin:
				case OpcodeType.DMin:
				case OpcodeType.Min:
					{
						CallHelper("min", token, 0, 1, 2);
						break;
					}
				//Resource Operations
				case OpcodeType.Gather4:
				case OpcodeType.Gather4S:
				case OpcodeType.Gather4Po:
				case OpcodeType.Gather4C:
				case OpcodeType.Gather4PoC:
				case OpcodeType.Gather4PoS:
				case OpcodeType.Gather4CS:
				case OpcodeType.Gather4PoCS:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleLS:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleDClS:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleCClS:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleBClS:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleClS:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleCLzS:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.Sample:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleL:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleC:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleCLz:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleD:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.SampleB:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.StoreRaw:
					{
						if(token.Operands[0].OperandType == OperandType.UnorderedAccessView)
						{
							AddCallInterfaceNoDest("Store", token, 0, 1, 2);
						}
						else
						{
							//TODO: use dstByteOffset (Operands[0]) to select the correct dest
							AddIndent();
							AddAssignToDest(token.Operands[0]);
							WriteOperand(token.Operands[2]);
							Output.AppendLine(";");
						}
						break;
					}
				case OpcodeType.StoreStructured:
					{
						AddCallInterfaceNoDest("Store", token, 0, 1, 2, 3);
						break;
					}
				case OpcodeType.StoreUavTyped:
					{
						AddCallInterfaceNoDest("Store", token, 0, 1, 2);
						break;
					}
				case OpcodeType.Ld:
					{
						AddLoad(token);
						break;
					}
				case OpcodeType.LdS:
					AddIndent();
					AddCallInterface("Load", token, 0, 2, 1, 3);
					break;
				case OpcodeType.LdMsS:
				case OpcodeType.LdMs:
					{
						AddCallInterface("LoadMs", token, 0, 2, 1);
						break;
					}
				case OpcodeType.LdStructuredS:
				case OpcodeType.LdStructured:
					{
						AddLoadStructured(token);
						break;
					}
				case OpcodeType.LdUavTypedS:
				case OpcodeType.LdUavTyped:
					{
						AddCallInterface("Load", token, 0, 2, 1);
						break;
					}
				case OpcodeType.LdRawS:
				case OpcodeType.LdRaw:
					{
						AddCallInterface("Load", token, 0, 2, 1);
						break;
					}
				case OpcodeType.Lod:
					{
						TranslateTextureSample(token);
						break;
					}
				case OpcodeType.Resinfo:
					{
						int parameterCount = token.Operands[0].GetNumSwizzleElements();
						for(int i = 0; i < 3 - parameterCount; i++)
						{
							AddIndent();
							Output.AppendLine($"float unused{i};");
						}
						AddIndent();
						WriteOperandWithMask(token.Operands[2], ComponentMask.None);
						Output.Append(".GetDimensions(");
						WriteOperand(token.Operands[1]);
						var usedMask = token.Operands[0].GetUsedComponents();
						for(int i = 0; i < 4; i++)
						{
							var mask = (ComponentMask)(1 << i);
							if(!usedMask.HasFlag(mask))
							{
								continue;
							}
							Output.Append(", ");
							WriteOperandWithMask(token.Operands[0], mask);
						}
						for(int i = 0; i < 3 - parameterCount; i++)
						{
							Output.Append($", unused{i}");
						}
						Output.AppendLine(");");
						break;
					}
				//Atomic Operations
				case OpcodeType.AtomicAnd:
					{
						AddCallInterfaceNoDest("InterlockedAnd", token, 0, 1, 2);
						break;
					}
				case OpcodeType.AtomicOr:
					{
						AddCallInterfaceNoDest("InterlockedOr", token, 0, 1, 2);
						break;
					}
				case OpcodeType.AtomicXor:
					{
						AddCallInterfaceNoDest("InterlockedXor", token, 0, 1, 2);
						break;
					}
				case OpcodeType.AtomicCmpStore:
					{
						AddCallInterfaceNoDest("InterlockedCompareStore", token, 0, 1, 2, 3);
						break;
					}
				case OpcodeType.AtomicIAdd:
					{
						AddCallInterfaceNoDest("InterlockedAdd", token, 0, 1, 2);
						break;
					}
				case OpcodeType.AtomicIMax:
					{
						AddCallInterfaceNoDest("InterlockedMax", token, 0, 1, 2);
						break;
					}
				case OpcodeType.AtomicIMin:
					{
						AddCallInterfaceNoDest("InterlockedMin", token, 0, 1, 2);
						break;
					}
				case OpcodeType.AtomicUMax:
					{
						AddCallInterfaceNoDest("InterlockedMax", token, 0, 1, 2);
						break;
					}
				case OpcodeType.AtomicUMin:
					{
						AddCallInterfaceNoDest("InterlockedMin", token, 0, 1, 2);
						break;
					}
				case OpcodeType.ImmAtomicIAdd:
					{
						AddCallInterfaceNoDest("InterlockedAnd", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicAnd:
					{
						AddCallInterfaceNoDest("InterlockedAnd", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicOr:
					{
						AddCallInterfaceNoDest("InterlockedOr", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicXor:
					{
						AddCallInterfaceNoDest("InterlockedXor", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicExch:
					{
						AddCallInterfaceNoDest("InterlockedExchange", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicCmpExch:
					{
						AddCallInterfaceNoDest("InterlockedCompareExchange", token, 1, 2, 3, 4, 0);
						break;
					}
				case OpcodeType.ImmAtomicIMax:
					{
						AddCallInterfaceNoDest("InterlockedMax", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicIMin:
					{
						AddCallInterfaceNoDest("InterlockedMin", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicUMax:
					{
						AddCallInterfaceNoDest("InterlockedMax", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicUMin:
					{
						AddCallInterfaceNoDest("InterlockedMin", token, 1, 2, 3, 0);
						break;
					}
				case OpcodeType.ImmAtomicAlloc:
					{
						AddCallInterfaceNoDest("Append", token, 1, 0);
						break;
					}
				case OpcodeType.ImmAtomicConsume:
					{
						AddCallInterface("Consume", token, 0, 1);
						break;
					}
				// Misc
				case OpcodeType.Nop:
					{
						break;
					}
				case OpcodeType.InterfaceCall:
					{
						AddIndent();
						var register = RegisterState.GetRegister(token);
						Output.AppendFormat("{0}();\n", register?.Name ?? token.ToString());
						break;
					}
				case OpcodeType.Emit:
					{
						AddIndent();
						Output.Append("TODO_Get_Stream");
						Output.AppendLine(".Append(output);");
						break;
					}
				case OpcodeType.Cut:
					{
						AddIndent();
						Output.Append("TODO_Get_Stream");
						Output.AppendLine(".RestartStrip();");
						break;
					}
				case OpcodeType.CutStream:
					{
						AddIndent();
						WriteOperand(token.Operands[0]);
						Output.AppendLine(".RestartStrip();");
						break;
					}
				case OpcodeType.EmitStream:
					{
						AddIndent();
						WriteOperand(token.Operands[0]);
						Output.AppendLine(".Append(output);");
						break;
					}
				case OpcodeType.EmitThenCutStream:
					{
						break;
					}
				case OpcodeType.Sync:
					{
						AddIndent();
						if(token.SyncFlags == SyncFlags.UnorderedAccessViewGlobal)
						{
							Output.AppendLine("DeviceMemoryBarrier();");
						}
						else if(token.SyncFlags == SyncFlags.SharedMemory)
						{
							Output.AppendLine("GroupMemoryBarrier();");
						}
						else if(token.SyncFlags == (SyncFlags.SharedMemory | SyncFlags.UnorderedAccessViewGlobal))
						{
							Output.AppendLine("AllMemoryBarrier();");
						}
						else if(token.SyncFlags == (SyncFlags.ThreadsInGroup | SyncFlags.UnorderedAccessViewGlobal))
						{
							Output.AppendLine("DeviceMemoryBarrierWithGroupSync();");
						}
						else if(token.SyncFlags == (SyncFlags.ThreadsInGroup | SyncFlags.SharedMemory))
						{
							Output.AppendLine("GroupMemoryBarrierWithGroupSync();");
						}
						else if(token.SyncFlags == (SyncFlags.ThreadsInGroup | SyncFlags.SharedMemory | SyncFlags.UnorderedAccessViewGlobal))
						{
							Output.AppendLine("AllMemoryBarrierWithGroupSync();");
						}
						else
						{
							throw new Exception($"Unknown Memory Sync Flags {token.SyncFlags}");
						}
						break;
					}
				case OpcodeType.Discard:
					{
						if(token.Operands[0].OperandType == OperandType.Immediate32 ||
							token.Operands[0].OperandType == OperandType.Immediate64)
						{
							AddIndent();
							Output.Append("discard;");
						}
						else
						{
							WriteConditional(token);
						}
						break;
					}
				case OpcodeType.EvalCentroid:
					{
						CallHelper("EvaluateAttributeCentroid", token, 0, 1);
						break;
					}
				case OpcodeType.EvalSampleIndex:
					{
						CallHelper("EvaluateAttributeAtSample", token, 0, 1, 2);
						break;
					}
				case OpcodeType.EvalSnapped:
					{
						CallHelper("EvaluateAttributeSnapped", token, 0, 1, 2);
						break;
					}
				case OpcodeType.Drcp:
				case OpcodeType.Rcp:
					{
						CallHelper("rcp", token, 0, 1);
						break;
					}
				case OpcodeType.F32ToF16:
					{
						CallHelper("f32tof16", token, 0, 1);
						break;
					}
				case OpcodeType.F16ToF32:
					{
						CallHelper("f16tof32", token, 0, 1);
						break;
					}
				case OpcodeType.INeg:
					{
						CallHelper("negate", token, 0, 1);
						break;
					}
				case OpcodeType.DerivRtx:
					{
						CallHelper("ddx", token, 0, 1);
						break;
					}
				case OpcodeType.RtxCoarse:
					{
						CallHelper("ddx_coarse", token, 0, 1);
						break;
					}
				case OpcodeType.RtxFine:
					{
						CallHelper("ddx_fine", token, 0, 1);
						break;
					}
				case OpcodeType.DerivRty:
					{
						CallHelper("ddy", token, 0, 1);
						break;
					}
				case OpcodeType.RtyCoarse:
					{
						CallHelper("ddy_coarse", token, 0, 1);
						break;
					}
				case OpcodeType.RtyFine:
					{
						CallHelper("ddy_fine", token, 0, 1);
						break;
					}
				case OpcodeType.SamplePos:
					{
						if(token.Operands[1].OperandType == OperandType.Rasterizer)
						{
							CallHelper("GetRenderTargetSamplePosition", token, 0, 2);
						}
						else
						{
							AddCallInterface("GetSamplePosition", token, 0, 1, 2);
						}
						break;
					}
				case OpcodeType.Abort:
					{
						AddIndent();
						Output.AppendLine("abort();");
						break;
					}
				case OpcodeType.Msad:
					CallHelper("msad4", token, 0, 1, 2);
					break;
				case OpcodeType.Bufinfo:
					//TODO: need to mask op[0] with no swizzle
					AddCallInterfaceNoDest("GetDimensions", token, 1, 0);
					break;
				case OpcodeType.FirstBitLo:
					CallHelper("firstbitlow", token, 0, 1);
					break;
				case OpcodeType.SampleInfo:
					AddIndent();
					AddAssignToDest(token.Operands[0]);
					Output.AppendLine("GetRenderTargetSampleCount();");
					break;
				case OpcodeType.HsForkPhase:
					break;
				case OpcodeType.HsJoinPhase:
					break;
				case OpcodeType.CheckAccessFullyMapped:
					CallHelper("CheckAccessFullyMapped", token, 0, 1);
					break;
				default:
					throw new Exception($"Unexpected token {token}");
			}
		}
		void CallHelper(string name, InstructionToken instruction, int dest, params int[] srcs)
		{
			AddIndent();
			WriteOperand(instruction.Operands[dest]);
			Output.AppendFormat(" = {0}(", name);
			for(int i = 0; i < srcs.Length; i++)
			{
				WriteOperand(instruction.Operands[srcs[i]]);
				if(i < srcs.Length - 1)
				{
					Output.Append(", ");
				}
			}
			Output.AppendLine(");");
		}
		void CallBinaryOp(string symbol, InstructionToken instruction, int dest, int src0, int src1)
		{
			AddIndent();
			WriteOperand(instruction.Operands[dest]);
			Output.Append(" = ");
			WriteOperand(instruction.Operands[src0]);
			Output.AppendFormat(" {0} ", symbol);
			WriteOperand(instruction.Operands[src1]);
			Output.AppendLine(";");
		}
		void WriteConditional(InstructionToken instruction)
		{
			AddIndent();
			Output.Append("if((");
			WriteOperand(instruction.Operands[0]);
			string statement = "";
			if(instruction.Header.OpcodeType == OpcodeType.BreakC)
			{
				statement = "break";
			}
			else if(instruction.Header.OpcodeType == OpcodeType.ContinueC)
			{
				statement = "continue";
			}
			else if(instruction.Header.OpcodeType == OpcodeType.RetC)
			{
				statement = "return";
			}
			else if(instruction.Header.OpcodeType == OpcodeType.Discard)
			{
				statement = "discard";
			}
			if(instruction.TestBoolean == InstructionTestBoolean.Zero)
			{
				if(instruction.Header.OpcodeType == OpcodeType.If)
				{
					Output.AppendLine(") == 0u){");
					indent++;
				}
				else
				{
					Output.AppendFormat(") == 0u){{ {0}; }}\n", statement);
				}
			}
			else
			{
				if(instruction.Header.OpcodeType == OpcodeType.If)
				{
					Output.AppendLine(") != 0u){");
					indent++;
				}
				else
				{
					Output.AppendFormat(") != 0u){{ {0}; }}\n", statement);
				}
			}
		}
		void WriteMoveBinaryOp(Operand dest, Operand source)
		{
			AddIndent();
			AddAssignToDest(dest);
			WriteOperand(source);
			Output.AppendLine(";");
		}
		void AddMOVCBinaryOp(Operand dest, Operand src0, Operand src1, Operand src2)
		{
			AddIndent();
			AddAssignToDest(dest);
			Output.Append("(");
			WriteOperand(src0);
			Output.Append(" >= 0) ? ");
			WriteOperand(src1);
			Output.Append(" : ");
			WriteOperand(src2);
			Output.AppendLine(";");
		}
		void AddAssignToDest(Operand dest)
		{
			WriteOperand(dest);
			Output.Append(" = ");
		}
		void AddComparision(InstructionToken inst, ComparisonType cmpType)
		{
			string[] ops = new string[] {
				"==",
				"<",
				">=",
				"!=",
			};
			AddIndent();
			AddAssignToDest(inst.Operands[0]);
			Output.Append("( ");

			WriteOperand(inst.Operands[1]);

			Output.AppendFormat(" {0} ", ops[(int)cmpType]);

			WriteOperand(inst.Operands[2]);

			Output.AppendLine(" ) ? 1.0 : 0.0;");
		}
		void CallTernaryOp(string op1, string op2, InstructionToken instruction, int dest, int src0, int src1, int src2)
		{
			AddIndent();
			AddAssignToDest(instruction.Operands[dest]);
			WriteOperand(instruction.Operands[src0]);
			Output.AppendFormat(" {0} ", op1);
			WriteOperand(instruction.Operands[src1]);
			Output.AppendFormat(" {0} ", op2);
			WriteOperand(instruction.Operands[src2]);
			Output.AppendLine(";");
		}
		string GetConstructorForType(ShaderVariableType type, int components)
		{
			string[] uintTypes = new string[] { " ", "uint", "uint2", "uint3", "uint4" };
			string[] intTypes = new string[] { " ", "int", "int2", "int3", "int4" };
			string[] floatTypes = new string[] { " ", "float", "float2", "float3", "float4" };
			string[] doubleTypes = new string[] { " ", "double", "double2", "double3", "double4" };
			if(components < 1 || components > 4)
				return "ERROR TOO MANY COMPONENTS IN VECTOR";
			switch(type)
			{
				case ShaderVariableType.UInt:
					return uintTypes[components];
				case ShaderVariableType.Int:
					return intTypes[components];
				case ShaderVariableType.Float:
					return floatTypes[components];
				case ShaderVariableType.Double:
					return doubleTypes[components];
				default:
					return $"ERROR UNSUPPORTED TYPE {type}";
			}
		}
		void AddCallInterface(string method, InstructionToken inst, int dest, int src0, params int[] args)
		{
			AddIndent();
			AddAssignToDest(inst.Operands[dest]);
			WriteOperandWithMask(inst.Operands[src0], ComponentMask.None);
			Output.AppendFormat(".{0}(", method);
			for(int i = 0; i < args.Length; i++)
			{
				if(i > 0) Output.Append(", ");
				WriteOperand(inst.Operands[args[i]]);
			}
			Output.AppendLine(");");
		}
		void AddCallInterfaceNoDest(string method, InstructionToken inst, int src0, params int[] args)
		{
			AddIndent();
			WriteOperandWithMask(inst.Operands[src0], ComponentMask.None);
			Output.AppendFormat(".{0}(", method);
			for(int i = 0; i < args.Length; i++)
			{
				if(i > 0) Output.Append(", ");
				WriteOperand(inst.Operands[args[i]]);
			}
			Output.AppendLine(");");
		}
		void TranslateTextureSample(InstructionToken inst)
		{
			AddIndent();
			AddAssignToDest(inst.Operands[0]);
			WriteOperandWithMask(inst.Operands[2], ComponentMask.None);
			switch(inst.Header.OpcodeType)
			{
				case OpcodeType.Sample:
					Output.Append(".Sample(");
					break;
				case OpcodeType.SampleBClS:
					Output.Append(".SampleBias(");
					break;
				case OpcodeType.SampleB:
					Output.Append(".SampleBias(");
					break;
				case OpcodeType.SampleClS:
					Output.Append(".SampleCmp(");
					break;
				case OpcodeType.SampleC:
					Output.Append(".SampleCmp(");
					break;
				case OpcodeType.SampleCLzS:
					Output.Append(".SampleCmpLevelZero(");
					break;
				case OpcodeType.SampleCLz:
					Output.Append(".SampleCmpLevelZero(");
					break;
				case OpcodeType.SampleDClS:
					Output.Append(".SampleGrad(");
					break;
				case OpcodeType.SampleD:
					Output.Append(".SampleGrad(");
					break;
				case OpcodeType.SampleLS:
					Output.Append(".SampleLevel(");
					break;
				case OpcodeType.SampleL:
					Output.Append(".SampleLevel(");
					break;
				case OpcodeType.SampleCClS:
					Output.Append(".Unknown(");
					break;
				case OpcodeType.Gather4:
				case OpcodeType.Gather4C:
				case OpcodeType.Gather4CS:
				case OpcodeType.Gather4Po:
				case OpcodeType.Gather4PoC:
				case OpcodeType.Gather4PoCS:
				case OpcodeType.Gather4PoS:
				case OpcodeType.Gather4S:
					Output.Append(".Gather4(");
					break;
				case OpcodeType.Lod:
					{
						Output.Append(".CalculateLevelOfDetail(");
						break;
					}
				case OpcodeType.Resinfo:
					Output.Append(".GetDimensions(");
					break;
				default:
					throw new InvalidOperationException($"Unexpected token {inst}");
			}
			WriteOperandWithMask(inst.Operands[3], ComponentMask.None);
			Output.Append(", ");
			WriteOperand(inst.Operands[1]);
			Output.AppendLine(");");
		}
		//Note:
		//result += tex.Load(index1).sValue2[0];
		//ld_structured_indexable(structured_buffer, stride=160)(mixed,mixed,mixed,mixed) o0.xyzw, cb0[0].x, l(80), t0.xyzw
		//dynamic indexing is translated into a series of conditional reads
		void AddLoadStructured(InstructionToken token)
		{
			AddIndent();
			AddAssignToDest(token.Operands[0]);
			var tex = token.Header.OpcodeType.IsFeedbackResourceAccess() ?
					token.Operands[4] :
					token.Operands[3];
			// tex.OperandType is Resource for texture reads, OperandType.UnorderedAccessView for UAV reads
			uint structSize = 16;
			if(tex.OperandType == OperandType.ThreadGroupSharedMemory)
			{
				var dcl = Container.Shader.DeclarationTokens
					.OfType<StructuredThreadGroupSharedMemoryDeclarationToken>()
					.First(d => d.Operand.Indices[0].Value == tex.Indices[0].Value);
				structSize = dcl.StructByteStride * dcl.StructCount * 4;
			}
			else
			{
				var buffer = GetConstantBuffer(tex.OperandType, (uint)tex.Indices[0].Value);
				structSize = buffer.Variables[0].Size;
			}
			Output.AppendFormat("((float4[{0}])", structSize / 16);
			WriteOperandWithMask(token.Operands[3], ComponentMask.None);
			Output.Append(".Load(");
			WriteOperand(token.Operands[1]);
			Output.AppendFormat("))[{0}]", token.Operands[2].ImmediateValues.GetNumber(0).UInt / 16);

			Output.AppendLine(";");
		}
		void AddLoad(InstructionToken token)
		{
			AddIndent();
			AddAssignToDest(token.Operands[0]);
			var tex = token.Operands[2];
			var resourceBinding = GetResourceBinding(tex.OperandType, (uint)tex.Indices[0].Value);
			if(resourceBinding.Type == ShaderInputType.TBuffer)
			{
				var buffer = GetConstantBuffer(tex.OperandType, (uint)tex.Indices[0].Value);
				var variable = buffer.Variables[0];
				bool castNeeded = variable.ShaderType.VariableClass != ShaderVariableClass.Vector
					|| variable.ShaderType.ElementCount < 2;
				if(castNeeded)
				{
					Output.AppendFormat("((float4[{0}])", buffer.Size / 16);
				}
				WriteOperandWithMask(tex, ComponentMask.None);
				if(castNeeded)
				{
					Output.Append(")");
				}
				Output.Append("[");
				if(token.Operands[1].OperandType == OperandType.Immediate32)
				{
					Output.Append(token.Operands[1].ImmediateValues.Int0);
				}
				else
				{
					WriteOperand(token.Operands[1]);
				}
				Output.Append("]");
				Output.AppendLine(";");
			}
			else
			{
				WriteOperandWithMask(tex, ComponentMask.None);
				Output.Append(".Load(");
				WriteOperand(token.Operands[1]);
				Output.Append(")");
				Output.AppendLine(";");
			}
		}

		void WriteShaderMessage(ShaderMessageDeclarationToken message)
		{
			AddIndent();
			Output.AppendFormat("// {0}", message.ToString());
			Output.AppendLine();
			AddIndent();
			string command;
			switch(message.InfoQueueMessageID)
			{
				case 2097410:
					command = "printf";
					break;
				case 2097411:
					command = "errorf";
					break;
				default:
					throw new NotSupportedException("Unknown InfoQueueMessageID: " + message.InfoQueueMessageID);
			}
			Output.AppendFormat("{0}(\"{1}\"", command, message.Format
				.Replace("\\", "\\\\")
				.Replace("\"", "\\\"")
				.Replace("\n", "\\n")
				.Replace("\t", "\\t")
				.Replace("\b", "\\b")
				.Replace("\r", "\\r")
				);

			foreach(var operand in message.Operands)
			{
				Output.AppendFormat(", ");
				WriteOperand(operand);
			}
			Output.AppendLine(");");
		}
	}
}
