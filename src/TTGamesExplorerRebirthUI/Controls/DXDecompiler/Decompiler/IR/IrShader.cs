using DXDecompiler.Decompiler.IR.ResourceDefinitions;

namespace DXDecompiler.Decompiler.IR
{
    public class IrShader
    {
        public List<IrPass> Passes = [];
        public IrEffect Effect;
        public IrResourceDefinition ResourceDefinition;
        public IrInterfaceManger InterfaceManger;
        public List<IrSignature> Signatures = [];
        public List<string> PreComments = [];
        public List<string> PostComments = [];
        public IrShader()
        {
        }
    }
}