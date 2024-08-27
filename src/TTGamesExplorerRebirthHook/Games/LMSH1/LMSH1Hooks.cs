using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TTGamesExplorerRebirthHook.Utils;

namespace TTGamesExplorerRebirthHook.Games.LMSH1
{
    public class LMSH1Hooks : ITTGames
    {
        private readonly TTGamesVersion _version;

        private string       _modsFolderPath;
        private List<string> _moddedFiles;

        private int _nuFileDeviceDatFileOpenOffset;
        private int _nuFileDeviceDatCreateNuFileOffset;
        private int _nuFileDeviceDatFileLoadBufferOffset;
        private int _nuFileDevicePCCreateNuFileOffset;

        private int _testOffset;

        private Hook<NuFileDeviceDat_FileOpen>       _nuFileDeviceDat_FileOpenHook;
        private Hook<NuFileDeviceDat_CreateNuFile>   _nuFileDeviceDat_CreateNuFileHook;
        private Hook<NuFileDeviceDat_FileLoadBuffer> _nuFileDeviceDat_FileLoadBufferHook;
        private Hook<NuFileDevicePC_CreateNuFile>    _nuFileDevicePC_CreateNuFileHook;

        public LMSH1Hooks(TTGamesVersion version)
        {
            _version = version;
        }

        public void Initialize()
        {
            if (_version == TTGamesVersion.LMSH1_Steamv04g)
            {
                Logger.Instance.Log("LEGOMarvel-26-09-13-Steam-v0.4g_Hotfix hooks loading...");

                _nuFileDeviceDatFileOpenOffset       = 0x1AC6C0;
                _nuFileDeviceDatCreateNuFileOffset   = 0x1B86A0;
                _nuFileDeviceDatFileLoadBufferOffset = 0x1B8860;
                _nuFileDevicePCCreateNuFileOffset    = 0x1B6170;
            }
            else if (_version == TTGamesVersion.LMSH1_SteamCallInPatchv03b)
            {
                Logger.Instance.Log("LEGOMarvel-10-02-14-Steam-Call-In-Patch-v0.3b_Hotfix hooks loading...");

                _nuFileDeviceDatFileOpenOffset       = 0x1AE9C0;
                _nuFileDeviceDatCreateNuFileOffset   = 0x1BA9B0;
                _nuFileDeviceDatFileLoadBufferOffset = 0x1BAB70;
                _nuFileDevicePCCreateNuFileOffset    = 0x1B8480;
            }

            _modsFolderPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "Mods");
            _moddedFiles    = new List<string>(Directory.GetFiles(_modsFolderPath, "*.*", SearchOption.AllDirectories));

            _nuFileDeviceDat_FileOpenHook       = new Hook<NuFileDeviceDat_FileOpen>      (NuFileDeviceDat_OpenImpl,           _nuFileDeviceDatFileOpenOffset);
            _nuFileDeviceDat_CreateNuFileHook   = new Hook<NuFileDeviceDat_CreateNuFile>  (NuFileDeviceDat_CreateNuFileImpl,   _nuFileDeviceDatCreateNuFileOffset);
            _nuFileDeviceDat_FileLoadBufferHook = new Hook<NuFileDeviceDat_FileLoadBuffer>(NuFileDeviceDat_FileLoadBufferImpl, _nuFileDeviceDatFileLoadBufferOffset);
            _nuFileDevicePC_CreateNuFileHook    = new Hook<NuFileDevicePC_CreateNuFile>   (NuFileDevicePC_CreateNuFileImpl,    _nuFileDevicePCCreateNuFileOffset);
        }

        // NuFileDeviceDat::FileOpen
        // int __thiscall sub_5AC6C0(void *this, _BYTE *a2, int a3)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_FileOpen(IntPtr thisPtr, IntPtr filePath, int nuFileMode);

        public int NuFileDeviceDat_OpenImpl(IntPtr thisPtr, IntPtr filePathPtr, int nuFileMode)
        {
            string filePath = filePathPtr.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            if (!filePath.StartsWith("host:"))
            {
                string moddedPath  = Path.Combine("Mods", filePath.Replace("/", "\\"));
                string matchedPath = _moddedFiles.Where(x => x.Contains(moddedPath)).FirstOrDefault();

                if (matchedPath != null)
                {
                    string finalModdedPath = $"host:{matchedPath}";

                    Logger.Instance.Log($"Modded -> {finalModdedPath}");

                    return _nuFileDeviceDat_FileOpenHook.OriginalFunction<int>(thisPtr, finalModdedPath.StringToPtr(), nuFileMode);
                }
            }

            return _nuFileDeviceDat_FileOpenHook.OriginalFunction<int>(thisPtr, filePathPtr, nuFileMode);
        }

        // NuFileDeviceDat::CreateNuFile
        // int __thiscall sub_5B86A0(void *this, int a2, int a3)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_CreateNuFile(IntPtr thisPtr, IntPtr filePath, int nuFileMode);

        private int NuFileDeviceDat_CreateNuFileImpl(IntPtr thisPtr, IntPtr filePathPtr, int unk)
        {
            string filePath = filePathPtr.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            if (filePath.StartsWith(_modsFolderPath))
            {
                Logger.Instance.Log($"Layered -> {filePath}");

                return _nuFileDevicePC_CreateNuFileHook.OriginalFunction<int>(filePathPtr, unk);
            }

            return _nuFileDeviceDat_CreateNuFileHook.OriginalFunction<int>(thisPtr, filePathPtr, unk);
        }

        // NuFileDeviceDat::FileLoadBuffer
        // int __stdcall sub_5B8860(int a1, int a2, int a3)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_FileLoadBuffer(IntPtr thisPtr, IntPtr filePath, IntPtr unk0, IntPtr unk1);

        private int NuFileDeviceDat_FileLoadBufferImpl(IntPtr thisPtr, IntPtr filePathPtr, IntPtr unk0, IntPtr unk1)
        {
            string filePath = filePathPtr.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            return _nuFileDeviceDat_FileLoadBufferHook.OriginalFunction<int>(thisPtr, filePathPtr, unk0, unk1);
        }

        // NuFileDevicePC::CreateNuFile
        // int __stdcall sub_5B6170(LPCSTR lpFileName, int a2)

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int NuFileDevicePC_CreateNuFile(IntPtr lpFileName, int unk);

        private int NuFileDevicePC_CreateNuFileImpl(IntPtr lpFileName, int unk)
        {
            string filePath = lpFileName.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            return _nuFileDevicePC_CreateNuFileHook.OriginalFunction<int>(lpFileName, unk);
        }
    }
}
