using System.Numerics;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
    public struct NuVertex
    {
        public NuVertexDescAttributeDefinition Definition;
        public NuVertexDescAttributeType       Type;
        public object                          Value;

        public readonly string ToObjString()
        {
            string s = "";
            
            if (Definition == NuVertexDescAttributeDefinition.Position)
            {
                s = "v ";
            }
            else if (Definition == NuVertexDescAttributeDefinition.Normal)
            {
                s = "vn ";
            }
            else if (Definition == NuVertexDescAttributeDefinition.UV01 || Definition == NuVertexDescAttributeDefinition.UV23)
            {
                s = "vt ";
            }

            if (Type == NuVertexDescAttributeType.Float1)
            {
                s += $"{(float)Value:0.00000000}".Replace(',', '.');
            }
            else if (Type == NuVertexDescAttributeType.Float2 || Type == NuVertexDescAttributeType.Half2)
            {
                s += $"{((Vector2)Value).X:0.00000000} {((Vector2)Value).Y:0.00000000}".Replace(',', '.');
            }
            else if (Type == NuVertexDescAttributeType.Float3)
            {
                s += $"{((Vector3)Value).X:0.00000000} {((Vector3)Value).Y:0.00000000} {((Vector3)Value).Z:0.00000000}".Replace(',', '.');
            }
            else if (Type == NuVertexDescAttributeType.Float4 || Type == NuVertexDescAttributeType.Half4 || Type == NuVertexDescAttributeType.UByteN4 || Type == NuVertexDescAttributeType.UByte4)
            {
                s += $"{((Vector4)Value).X:0.00000000} {((Vector4)Value).Y:0.00000000} {((Vector4)Value).Z:0.00000000} {((Vector4)Value).W:0.00000000}".Replace(',', '.');
            }
            else if (Type == NuVertexDescAttributeType.Color)
            {
                // TODO: Not supported yet.
                // s += $"{((Rgba32)Value).R} {((Rgba32)Value).G} {((Rgba32)Value).B} {((Rgba32)Value).A}";
            }

            return s;
        }
    }
}