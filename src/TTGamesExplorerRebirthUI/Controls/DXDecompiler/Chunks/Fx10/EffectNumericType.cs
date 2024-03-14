﻿using DXDecompiler.Util;

namespace DXDecompiler.Chunks.Fx10
{
	public class EffectNumericType
	{
		/// <summary>
		/// scalar (1x1), vector (1xN), matrix (NxN)
		/// </summary>
		public EffectNumericLayout NumericLayout { get; private set; }
		/// <summary>
		/// float32, int32, int8, etc.
		/// </summary>
		public EffectScalarType ScalarType { get; private set; }
		/// <summary>
		/// 1 <= Rows <= 4
		/// </summary>
		public uint Rows { get; private set; }
		/// <summary>
		/// 1 <= Columns <= 4
		/// </summary>
		public uint Columns { get; private set; }
		/// <summary>
		/// applies only to matrices
		/// </summary>
		public bool IsColumnMajor { get; private set; }
		/// <summary>
		/// if this is an array, indicates whether elements should be greedily packed
		/// </summary>
		public bool IsPackedArray { get; private set; }

		public static EffectNumericType Parse(uint type)
		{
			return new EffectNumericType()
			{
				NumericLayout = (EffectNumericLayout)type.DecodeValue(0, 1),
				ScalarType = (EffectScalarType)type.DecodeValue(3, 5),
				Rows = type.DecodeValue(8, 10),
				Columns = type.DecodeValue(11, 13),
				IsColumnMajor = type.DecodeValue(14, 14) != 0,
				IsPackedArray = type.DecodeValue(15, 15) != 0,
			};
		}
	}
}
