using System.Runtime.InteropServices;

namespace TTGamesExplorerRebirthUI
{
    public static partial class Natives
    {
        public enum HResult : int
        {
            S_OK
        }

        [LibraryImport("DwmApi")]
        public static partial HResult DwmSetWindowAttribute(IntPtr hwnd, int attr, [In] int[] attrValue, int attrSize);
    }
}