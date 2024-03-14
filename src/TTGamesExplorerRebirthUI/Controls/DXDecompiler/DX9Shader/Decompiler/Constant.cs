﻿namespace DXDecompiler.DX9Shader
{
	public class Constant
	{
		public uint RegisterIndex { get; private set; }
		public float[] Value { get; private set; }

		public Constant(uint registerIndex, float value0, float value1, float value2, float value3)
		{
			RegisterIndex = registerIndex;
			Value = new[] { value0, value1, value2, value3 };
		}

		public float this[int index]
		{
			get
			{
				return Value[index];
			}
			set
			{
				Value[index] = value;
			}
		}
	}

	public class ConstantInt
	{
		public uint RegisterIndex { get; private set; }
		public uint[] Value { get; private set; }

		public ConstantInt(uint registerIndex, uint value0, uint value1, uint value2, uint value3)
		{
			RegisterIndex = registerIndex;
			Value = new[] { value0, value1, value2, value3 };
		}

		public uint this[int index]
		{
			get
			{
				return Value[index];
			}
			set
			{
				Value[index] = value;
			}
		}
	}
}
