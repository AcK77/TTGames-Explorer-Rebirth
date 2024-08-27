using System;
using System.Runtime.InteropServices;

namespace TTGamesExplorerRebirthHook.Utils
{
    public static class Natives
    {
        public enum VirtualProtectionType : uint
        {
            Execute                  = 0x10,
            ExecuteRead              = 0x20,
            ExecuteReadWrite         = 0x40,
            ExecuteWriteCopy         = 0x80,
            NoAccess                 = 0x01,
            Readonly                 = 0x02,
            ReadWrite                = 0x04,
            WriteCopy                = 0x08,
            GuardModifierflag        = 0x100,
            NoCacheModifierflag      = 0x200,
            WriteCombineModifierflag = 0x400,
        }

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, VirtualProtectionType flNewProtect, out VirtualProtectionType lpflOldProtect);
    }
}