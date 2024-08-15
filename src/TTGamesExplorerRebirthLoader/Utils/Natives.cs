using System.Runtime.InteropServices;

namespace TTGamesExplorerRebirthLoader.Utils
{
    public static class Natives
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern nint VirtualAllocEx(nint hProcess, nint lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, uint nSize, out nuint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern nint GetProcAddress(nint hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint CreateRemoteThread(
            nint hProcess,
            nint lpThreadAttribute,
            nint dwStackSize,
            nint lpStartAddress,
            nint lpParameter,
            uint dwCreationFlags,
            nint lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CreateProcessW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpApplicationName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpCommandLine,
            ref SecurityAttributes lpProcessAttributes,
            ref SecurityAttributes lpThreadAttributes,
            bool bInheritHandles,
            ProcessCreationFlags dwCreationFlags,
            nint lpEnvironment,
            [MarshalAs(UnmanagedType.LPWStr)] string lpCurrentDirectory,
            ref StartupInformation lpStartupInfo,
            ref ProcessInformation lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(nint hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern nint LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(nint hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint CreateToolhelp32Snapshot(int flags, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Module32First(nint hSnapshot, ref ModuleEntry32 lpme);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Module32Next(nint hSnapshot, ref ModuleEntry32 lpme);

        // Constants

        public const int PROCESS_CREATE_THREAD = 0x0002;
        public const int PROCESS_QUERY_INFORMATION = 0x0400;
        public const int PROCESS_VM_OPERATION = 0x0008;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_ALL_ACCESS = 0x001F0FFF;

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000,
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400,
        }

        [Flags]
        public enum ProcessCreationFlags : int
        {
            ZERO_FLAG = 0x00000000,
            CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,
            CREATE_NEW_CONSOLE = 0x00000010,
            CREATE_NEW_PROCESS_GROUP = 0x00000200,
            CREATE_NO_WINDOW = 0x08000000,
            CREATE_PROTECTED_PROCESS = 0x00040000,
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
            CREATE_SEPARATE_WOW_VDM = 0x00001000,
            CREATE_SHARED_WOW_VDM = 0x00001000,
            CREATE_SUSPENDED = 0x00000004,
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,
            DEBUG_ONLY_THIS_PROCESS = 0x00000002,
            DEBUG_PROCESS = 0x00000001,
            DETACHED_PROCESS = 0x00000008,
            EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
            INHERIT_PARENT_AFFINITY = 0x00010000,
        }

        public struct SecurityAttributes
        {
            public int nLength;
            public nint lpSecurityDescriptor;
            public int bInheritHandle;
        }

        public struct StartupInformation
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public nint lpReserved2;
            public nint hStdInput;
            public nint hStdOutput;
            public nint hStdError;
        }

        public struct ProcessInformation
        {
            public nint hProcess;
            public nint hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        public struct ModuleEntry32
        {
            public uint Size;
            public uint Th32ModuleID;
            public uint Th32ProcessID;
            public uint GlblcntUsage;
            public uint ProccntUsage;
            public nint ModBaseAddr;
            public uint ModBaseSize;
            public nint HModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string ModuleName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string ExePath;
        };
    }
}