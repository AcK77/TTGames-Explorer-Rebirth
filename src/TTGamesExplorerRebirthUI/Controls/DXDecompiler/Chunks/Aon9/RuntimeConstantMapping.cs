﻿using DXDecompiler.Util;
using System.Diagnostics;

namespace DXDecompiler.Chunks.Aon9
{
    public class RuntimeConstantMapping
    {
        public ushort TargetReg { get; private set; }
        public RuntimeConstantDescription ConstantDescription { get; private set; }

        public static RuntimeConstantMapping Parse(BytecodeReader reader)
        {
            var result = new RuntimeConstantMapping
            {
                ConstantDescription = (RuntimeConstantDescription)reader.ReadUInt16(),
                TargetReg = reader.ReadUInt16()
            };

            Debug.Assert(Enum.IsDefined(typeof(RuntimeConstantDescription), result.ConstantDescription), $"Unknown RuntimeConstantDescription {result.ConstantDescription}");

            return result;
        }

        public override string ToString()
        {
            return string.Format("// c{0, -9} {1, 50}", TargetReg, ConstantDescription.GetDescription());
        }
    }
}