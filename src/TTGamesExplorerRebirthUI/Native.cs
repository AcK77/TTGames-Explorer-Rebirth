using System.Runtime.InteropServices;

namespace TTGamesExplorerRebirthUI
{
    public static partial class Native
    {
        public enum HResult : int
        {
            S_OK
        }

        [LibraryImport("DwmApi")]
        public static partial HResult DwmSetWindowAttribute(IntPtr hwnd, int attr, [In] int[] attrValue, int attrSize);
    }
}