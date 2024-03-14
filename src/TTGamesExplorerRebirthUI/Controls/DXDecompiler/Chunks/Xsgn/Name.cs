﻿using DXDecompiler.Chunks.Shex;

namespace DXDecompiler.Chunks.Xsgn
{
    /// <summary>
    /// Values that identify shader parameters that use system-value semantics.
    /// Based on D3D_NAME. This is similar to, but different from, <see cref="SystemValueName"/>
    /// </summary>
    public enum Name
    {
        [Description("NONE")]
        Undefined = 0,

        [Description("POS")]
        Position = 1,

        [Description("CLIPDST")]
        ClipDistance = 2,

        [Description("CULLDST")]
        CullDistance = 3,

        [Description("RTINDEX")]
        RenderTargetArrayIndex = 4,

        [Description("VPINDEX")]
        ViewportArrayIndex = 5,

        [Description("VERTID")]
        VertexID = 6,

        [Description("PRIMID")]
        PrimitiveID = 7,

        [Description("INSTID")]
        InstanceID = 8,

        [Description("FFACE")]
        IsFrontFace = 9,

        [Description("SAMPLE")]
        SampleIndex = 10,

        [Description("QUADEDGE")]
        FinalQuadEdgeTessFactor = 11,

        [Description("QUADINT")]
        FinalQuadInsideTessFactor = 12,

        [Description("TRIEDGE")]
        FinalTriEdgeTessFactor = 13,

        [Description("TRIINT")]
        FinalTriInsideTessFactor = 14,

        [Description("LINEDET")]
        FinalLineDetailTessFactor = 15,

        [Description("LINEDEN")]
        FinalLineDensityTessFactor = 16,

        [Description("BARYCEN")]
        Barycentrics = 23,

        [Description("SHDINGRATE")]
        ShadingRate = 24,

        [Description("CULLPRIM")]
        CullPrimitive = 25,

        [Description("TARGET")]
        Target = 64,

        [Description("DEPTH")]
        Depth = 65,

        [Description("COVERAGE")]
        Coverage = 66,

        [Description("DEPTHGE")]
        DepthGreaterEqual = 67,

        [Description("DEPTHLE")]
        DepthLessEqual = 68,

        [Description("STENCILREF")]
        StencilRef = 69,

        [Description("INNERCOVERAGE")]
        InnerCoverage = 70,
    }
}