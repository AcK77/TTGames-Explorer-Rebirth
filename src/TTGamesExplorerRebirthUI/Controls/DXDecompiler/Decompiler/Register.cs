namespace DXDecompiler.Decompiler
{
    public class Register(string name)
    {
        public string Name { get; private set; } = name;

        public override string ToString()
        {
            return Name;
        }
    }
}