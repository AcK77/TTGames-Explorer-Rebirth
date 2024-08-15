using System;
using System.Runtime.InteropServices;

namespace TTGamesExplorerRebirthHook.Utils
{
    public static class IntPtrExt
    {
        public static string PtrToString(this IntPtr ptr, int offset = 0)
        {
            return Marshal.PtrToStringAnsi(new IntPtr(ptr.ToInt32() + offset));
        }

        public static string PtrToPtrToString(this IntPtr ptr, int offset = 0)
        {
            return Marshal.PtrToStringAnsi(new IntPtr(Marshal.ReadIntPtr(ptr).ToInt32() + offset));
        }

        public static IntPtr StringToPtr(this string text)
        {
            return Marshal.StringToHGlobalAnsi(text);
        }
    }
}