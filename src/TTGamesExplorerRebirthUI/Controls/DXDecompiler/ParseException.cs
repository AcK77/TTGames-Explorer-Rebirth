namespace DXDecompiler
{
    public class ParseException(string message) : Exception(message)
    {
        public ParseException(string format, params object[] args) : this(string.Format(format, args))
        {
        }
    }
}