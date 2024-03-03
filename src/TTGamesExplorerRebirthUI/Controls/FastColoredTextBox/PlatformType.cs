using System.Runtime.InteropServices;

namespace FastColoredTextBoxNS
{
    public static partial class PlatformType
    {
        const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
        const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
        const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;

        [StructLayout(LayoutKind.Sequential)]
        struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        };

        [LibraryImport("kernel32.dll")]
        static partial void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        [LibraryImport("kernel32.dll")]
        static partial void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        public static Platform GetOperationSystemPlatform()
        {
            var sysInfo = new SYSTEM_INFO();

            // WinXP and older - use GetNativeSystemInfo
            if (Environment.OSVersion.Version.Major > 5 || (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1))
            {
                GetNativeSystemInfo(ref sysInfo);
            }
            else
            {
                GetSystemInfo(ref sysInfo);
            }

            return sysInfo.wProcessorArchitecture switch
            {
                PROCESSOR_ARCHITECTURE_IA64 or PROCESSOR_ARCHITECTURE_AMD64 => Platform.X64,
                PROCESSOR_ARCHITECTURE_INTEL => Platform.X86,
                _ => Platform.Unknown,
            };
        }
    }

    public enum Platform
    {
        X86,
        X64,
        Unknown,
    }
}