using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TTGamesExplorerRebirthLoader.Utils;
using static TTGamesExplorerRebirthLoader.Utils.Natives;

namespace TTGamesExplorerRebirthLoader
{
    internal class Program
    {
        private static int _pid;

        static void Main(string[] args)
        {
            Logger logger = new();

            logger.ReadingCompleted += Logger_ReadingCompleted;

            RunProcess(args[0]);

            Inject32Bit(args[0], args[1], args[2]);

            Console.ReadKey();

            logger.Dispose();
        }

        private static void Logger_ReadingCompleted(object sender, string message)
        {
            Console.WriteLine(message);
        }

        public static void RunProcess(string exePath)
        {
            StartupInformation startupInformation  = new();
            SecurityAttributes lpProcessAttributes = new();
            SecurityAttributes lpThreadAttributes  = new();
            ProcessInformation processInformation  = new();

            bool success = CreateProcessW(null, exePath, ref lpProcessAttributes, ref lpThreadAttributes, false, ProcessCreationFlags.CREATE_NEW_PROCESS_GROUP, IntPtr.Zero, Path.GetDirectoryName(exePath), ref startupInformation, ref processInformation);
            if (!success)
            {
                throw new NullReferenceException($"Error at \"CreateProcessW\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            _pid = (int)processInformation.dwProcessId;
        }

        static void Inject32Bit(string exePath, string dllPath, string dllHookPath)
        {
            IntPtr hLoaded  = LoadLibrary(dllPath);
            IntPtr lpInject = GetProcAddress(hLoaded, "ImplantDotNetAssembly"); // Get address of function to invoke
            IntPtr offset   = new(lpInject.ToInt64() - hLoaded.ToInt64());      // Compute the distance between the base address and the function to invoke

            // Unload library from this process
            FreeLibrary(hLoaded);

            Process[] localByName   = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exePath));
            byte[]    dllPathBuffer = Encoding.ASCII.GetBytes(dllPath);

            _pid = _pid == -1 ? localByName[0].Id : _pid;

            IntPtr hProcess = OpenProcess(PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_CREATE_THREAD, false, _pid);
            if (hProcess == IntPtr.Zero)
            {
                throw new NullReferenceException($"Error at \"OpenProcess\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            Thread.Sleep(1000);

            IntPtr allocatedMemoryAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)dllPathBuffer.Length + 1, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            if (allocatedMemoryAddress == IntPtr.Zero)
            {
                throw new NullReferenceException($"Error at \"VirtualAllocEx\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            if (!WriteProcessMemory(hProcess, allocatedMemoryAddress, dllPathBuffer, (uint)dllPathBuffer.Length, out _))
            {
                throw new NullReferenceException($"Error at \"WriteProcessMemory\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            IntPtr loadLibAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (loadLibAddress == IntPtr.Zero)
            {
                throw new NullReferenceException($"Error at \"GetProcAddress\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibAddress, allocatedMemoryAddress, 0, IntPtr.Zero);
            if (hThread == IntPtr.Zero)
            {
                throw new NullReferenceException($"Error at \"CreateRemoteThread\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            ManualResetEvent gameProcessFoundEvent = new(false);
            bool gameProcessFound = false;

            new Thread(() =>
            {
                while (!gameProcessFound)
                {
                    foreach (var process in Process.GetProcesses())
                    {
                        if (process.ProcessName == Path.GetFileNameWithoutExtension(exePath))
                        {
                            gameProcessFound = true;
                            gameProcessFoundEvent.Set();
                            break;
                        }
                    }
                }

            }).Start();

            gameProcessFoundEvent.WaitOne();
            gameProcessFound = true;

            Thread.Sleep(1000);

            byte[] loadedDllFolderPathBuffer = Encoding.ASCII.GetBytes(dllHookPath);

            allocatedMemoryAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)loadedDllFolderPathBuffer.Length + 1, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            if (allocatedMemoryAddress == IntPtr.Zero)
            {
                throw new NullReferenceException($"Error at \"VirtualAllocEx\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            if (!WriteProcessMemory(hProcess, allocatedMemoryAddress, loadedDllFolderPathBuffer, (uint)loadedDllFolderPathBuffer.Length, out _))
            {
                throw new NullReferenceException($"Error at \"WriteProcessMemory\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            IntPtr bootstrapAddress = GetRemoteModuleHandle(_pid, dllPath);

            hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, new IntPtr(bootstrapAddress.ToInt64() + offset.ToInt64()), allocatedMemoryAddress, 0, IntPtr.Zero);
            if (hThread == IntPtr.Zero)
            {
                throw new NullReferenceException($"Error at \"CreateRemoteThread\": {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }
        }

        public static IntPtr GetRemoteModuleHandle(int processId, string moduleName)
        {
            IntPtr pointer = CreateToolhelp32Snapshot(0x8, processId);

            ModuleEntry32 moduleEntry32 = new();
            moduleEntry32.Size = (uint)Marshal.SizeOf(moduleEntry32);

            if (Module32First(pointer, ref moduleEntry32))
            {
                do
                {
                    if (moduleEntry32.ModuleName == Path.GetFileName(moduleName))
                    {
                        break;
                    }

                    moduleEntry32.Size = (uint)Marshal.SizeOf(moduleEntry32);
                } while (Module32Next(pointer, ref moduleEntry32));
            }

            if (moduleEntry32.ModuleName != Path.GetFileName(moduleName))
            {
                throw new DllNotFoundException(Path.GetFileName(moduleName));
            }

            CloseHandle(pointer);

            return moduleEntry32.ModBaseAddr;
        }
    }
}