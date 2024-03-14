﻿using System;

namespace DXDecompiler.Chunks.Sfi0
{
	[Flags]
	public enum ShaderRequiresFlags
	{
		None = 0,

		/// <summary>
		/// Shader requires that the graphics driver and hardware support double data type.
		/// </summary>
		Doubles = 0x1,

		/// <summary>
		/// Shader requires an early depth stencil.
		/// </summary>
		EarlyDepthStencil = 0x2,

		/// <summary>
		/// Shader requires unordered access views (UAVs) at every pipeline stage.
		/// </summary>
		UavsAtEveryStage = 0x4,

		/// <summary>
		/// Shader requires 64 UAVs.
		/// </summary>
		Requires64Uavs = 0x8,

		/// <summary>
		/// Shader requires the graphics driver and hardware to support minimum precision.
		/// </summary>
		MinimumPrecision = 0x10,

		/// <summary>
		/// Shader requires that the graphics driver and hardware support extended doubles instructions.
		/// </summary>
		DoubleExtensionsFor11Point1 = 0x20,


		/// <summary>
		/// Shader requires that the graphics driver and hardware support the msad4 intrinsic function in shaders.
		/// </summary>
		ShaderExtensionsFor11Point1 = 0x40,

		/// <summary>
		/// Shader requires that the graphics driver and hardware support Direct3D 9 shadow support.
		/// </summary>
		Level9ComparisonFiltering = 0x80,

		/// <summary>
		/// Shader requires that the graphics driver and hardware support tiled resources.
		/// </summary>
		TiledResources = 0x100,

		/// <summary>
		/// Shader requires a reference value for depth stencil tests.
		/// </summary>
		StencilRef = 0x200,

		/// <summary>
		/// Shader requires that the graphics driver and hardware support inner coverage.
		/// </summary>
		InnerCoverage = 0x400,

		/// <summary>
		/// Shader requires that the graphics driver and hardware support the loading of additional formats for typed unordered-access views (UAVs).
		/// </summary>
		TypedUAVLoadAdditionalFormats = 0x800,

		/// <summary>
		/// Shader requires that the graphics driver and hardware support rasterizer ordered views (ROVs).
		/// </summary>
		Rovs = 0x1000,

		/// <summary>
		/// Shader requires that the graphics driver and hardware support viewport and render target array index values from any shader-feeding rasterizer.
		/// </summary>
		SVArrayIndexFromFeedingRasterizer = 0x2000,

		WaveOps = 0x4000,
		Int64Ops = 0x8000,
		ViewID = 0x10000,
		Barycentrics = 0x20000,
		NativeLowPrecision = 0x40000,
		ShadingRate = 0x80000,
		Raytracing_Tier_1_1 = 0x100000,
		SamplerFeedback = 0x200000,
		AtomicInt64OnTypedResource = 0x400000,
		AtomicInt64OnGroupShared = 0x800000,
		DerivativesInMeshAndAmpShaders = 0x1000000
	}
}
