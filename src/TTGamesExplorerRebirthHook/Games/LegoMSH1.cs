using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TTGamesExplorerRebirthHook.Utils;

namespace TTGamesExplorerRebirthHook.Games
{
    public class LegoMSH1
    {
        private string _modsFolderPath;
        private readonly List<string> _moddedFiles;

        // NuFileDeviceDat::FileOpen
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_FileOpen(IntPtr thisPtr, IntPtr filePath, int nuFileMode);
        private readonly Hook _nuFileDeviceDat_FileOpenFuncHook = new Hook();
        private readonly NuFileDeviceDat_FileOpen _nuFileDeviceDat_FileOpen;

        // NuFileDeviceDat::CreateNuFile
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_CreateNuFile(IntPtr thisPtr, IntPtr filePath, int nuFileMode);
        private readonly Hook _nuFileDeviceDat_CreateNuFileFuncHook = new Hook();
        private readonly NuFileDeviceDat_CreateNuFile _nuFileDeviceDat_CreateNuFile;

        // NuFileDeviceDat::FileLoadBuffer
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_FileLoadBuffer(IntPtr thisPtr, IntPtr filePath, IntPtr unk0, IntPtr unk1);
        private readonly Hook _nuFileDeviceDat_FileLoadBufferFuncHook = new Hook();
        private readonly NuFileDeviceDat_FileLoadBuffer _nuFileDeviceDat_FileLoadBuffer;

        // NuFileDevicePC::CreateNuFile
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int NuFileDevicePC_CreateNuFile(IntPtr lpFileName, int unk);
        private readonly Hook _nuFileDevicePC_CreateNuFileFuncHook = new Hook();
        private readonly NuFileDevicePC_CreateNuFile _nuFileDevicePC_CreateNuFile;

        public LegoMSH1(string version)
        {
            int nuFileDeviceDat_FileOpen_Offset = 0;
            int nuFileDeviceDat_CreateNuFile_Offset = 0;
            int nuFileDeviceDat_FileLoadBuffer_Offset = 0;
            int nuFileDevicePC_CreateNuFile_Offset = 0;

            if (version == "26-09-13")
            {
                nuFileDeviceDat_FileOpen_Offset = 0x1AC6C0;
                nuFileDeviceDat_CreateNuFile_Offset = 0x1B86A0;
                nuFileDeviceDat_FileLoadBuffer_Offset = 0x1B8860;
                nuFileDevicePC_CreateNuFile_Offset = 0x1B6170;
            }
            else if (version == "10-02-14")
            {
                nuFileDeviceDat_FileOpen_Offset = 0x1AE9C0;
                nuFileDeviceDat_CreateNuFile_Offset = 0x1BA9B0;
                nuFileDeviceDat_FileLoadBuffer_Offset = 0x1BAB70;
                nuFileDevicePC_CreateNuFile_Offset = 0x1B8480;
            }

            _modsFolderPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "Mods");
            _moddedFiles = new List<string>(Directory.GetFiles(_modsFolderPath, "*.*", SearchOption.AllDirectories));

            _nuFileDeviceDat_FileOpen = _nuFileDeviceDat_FileOpenFuncHook.CreateHook<NuFileDeviceDat_FileOpen>(NuFileDeviceDat_OpenImpl, nuFileDeviceDat_FileOpen_Offset);
            _nuFileDeviceDat_CreateNuFile = _nuFileDeviceDat_CreateNuFileFuncHook.CreateHook<NuFileDeviceDat_CreateNuFile>(NuFileDeviceDat_CreateNuFileImpl, nuFileDeviceDat_CreateNuFile_Offset);
            _nuFileDeviceDat_FileLoadBuffer = _nuFileDeviceDat_FileLoadBufferFuncHook.CreateHook<NuFileDeviceDat_FileLoadBuffer>(NuFileDeviceDat_FileLoadBufferImpl, nuFileDeviceDat_FileLoadBuffer_Offset);

            _nuFileDevicePC_CreateNuFile = _nuFileDevicePC_CreateNuFileFuncHook.CreateHook<NuFileDevicePC_CreateNuFile>(NuFileDevicePC_CreateNuFileImpl, nuFileDevicePC_CreateNuFile_Offset);
        }

        // int __thiscall sub_5AC6C0(void *this, _BYTE *a2, int a3)
        private int NuFileDeviceDat_OpenImpl(IntPtr thisPtr, IntPtr filePathPtr, int nuFileMode)
        {
            string filePath = filePathPtr.PtrToString();

            Logger.Instance.Log($"{filePath}");

            if (!filePath.StartsWith("host:"))
            {
                string moddedPath = Path.Combine("Mods", filePath.Replace("/", "\\"));
                string matchedPath = _moddedFiles.Where(x => x.Contains(moddedPath)).FirstOrDefault();

                if (matchedPath != null)
                {
                    string finalModdedPath = $"host:{matchedPath}";

                    Logger.Instance.Log($"Modded -> {finalModdedPath}");

                    return _nuFileDeviceDat_FileOpenFuncHook.OriginalFunction<int, NuFileDeviceDat_FileOpen>(thisPtr, finalModdedPath.StringToPtr(), nuFileMode);
                }
            }

            return _nuFileDeviceDat_FileOpenFuncHook.OriginalFunction<int, NuFileDeviceDat_FileOpen>(thisPtr, filePathPtr, nuFileMode);
        }

        // int __thiscall sub_5B86A0(void *this, int a2, int a3)
        private int NuFileDeviceDat_CreateNuFileImpl(IntPtr thisPtr, IntPtr filePathPtr, int unk)
        {
            string filePath = filePathPtr.PtrToString();

            Logger.Instance.Log($"{filePath}");

            if (filePath.StartsWith(_modsFolderPath))
            {
                Logger.Instance.Log($"Layered -> {filePath}");

                return _nuFileDevicePC_CreateNuFileFuncHook.OriginalFunction<int, NuFileDevicePC_CreateNuFile>(filePathPtr, unk);
            }

            return _nuFileDeviceDat_CreateNuFileFuncHook.OriginalFunction<int, NuFileDeviceDat_CreateNuFile>(thisPtr, filePathPtr, unk);
        }

        // int __stdcall sub_5B8860(int a1, int a2, int a3)
        private int NuFileDeviceDat_FileLoadBufferImpl(IntPtr thisPtr, IntPtr filePathPtr, IntPtr unk0, IntPtr unk1)
        {
            string filePath = filePathPtr.PtrToString();

            Logger.Instance.Log($"{filePath}");

            return _nuFileDeviceDat_FileLoadBufferFuncHook.OriginalFunction<int, NuFileDeviceDat_FileLoadBuffer>(thisPtr, filePathPtr, unk0, unk1);
        }

        // int __stdcall sub_5B6170(LPCSTR lpFileName, int a2)
        private int NuFileDevicePC_CreateNuFileImpl(IntPtr lpFileName, int unk)
        {
            string filePath = lpFileName.PtrToString();

            Logger.Instance.Log($"{filePath}");

            return _nuFileDevicePC_CreateNuFileFuncHook.OriginalFunction<int, NuFileDevicePC_CreateNuFile>(lpFileName, unk);
        }
    }
}
