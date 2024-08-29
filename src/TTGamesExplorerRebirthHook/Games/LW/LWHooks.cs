using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TTGamesExplorerRebirthHook.Utils;

namespace TTGamesExplorerRebirthHook.Games.LW
{
    public class LWHooks : ITTGames
    {
        private readonly TTGamesVersion _version;

        private string       _modsFolderPath;
        private List<string> _moddedFiles;

        private int _nuFileDeviceDatCreateNuFileOffset;
        private int _nuFileDevicePCCreateNuFileOffset;

        private Hook<NuFileDeviceDat_CreateNuFile> _nuFileDeviceDat_CreateNuFileHook;
        private Hook<NuFileDevicePC_CreateNuFile>  _nuFileDevicePC_CreateNuFileHook;

        public LWHooks(TTGamesVersion version)
        {
            _version = version;
        }

        public void Initialize()
        {
            if (_version == TTGamesVersion.LW_TU3Dv01)
            {
                Logger.Instance.Log("BADGER-07-12-18-TU3D-v0.1_Hotfix hooks loading...");

                _nuFileDeviceDatCreateNuFileOffset = 0x17D5D0;
                _nuFileDevicePCCreateNuFileOffset  = 0x17D7B0;
            }

            _modsFolderPath = "Mods";
            _moddedFiles    = new List<string>(Directory.GetFiles(_modsFolderPath, "*.*", SearchOption.AllDirectories));

            _nuFileDeviceDat_CreateNuFileHook = new Hook<NuFileDeviceDat_CreateNuFile> (NuFileDeviceDat_CreateNuFileImpl, _nuFileDeviceDatCreateNuFileOffset);
            _nuFileDevicePC_CreateNuFileHook  = new Hook<NuFileDevicePC_CreateNuFile>  (NuFileDevicePC_CreateNuFileImpl,  _nuFileDevicePCCreateNuFileOffset);
        }

        // NuFileDeviceDat::CreateNuFile
        // _DWORD *__thiscall sub_57D5D0(char *this, int a2, int a3, int a4)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr NuFileDeviceDat_CreateNuFile(IntPtr thisPtr, IntPtr unk0, IntPtr filePathPtr, IntPtr unk1);

        private IntPtr NuFileDeviceDat_CreateNuFileImpl(IntPtr thisPtr, IntPtr unk0, IntPtr filePathPtr, IntPtr unk1)
        {
            string filePath = filePathPtr.PtrToPtrToString();

            // Logger.Instance.Log($"{filePath}");

            string moddedPath  = Path.Combine("Mods", filePath.Replace("/", "\\"));
            string matchedPath = _moddedFiles.Where(x => x.Contains(moddedPath)).FirstOrDefault();

            if (matchedPath != null)
            {
                Logger.Instance.Log($"Modded -> {matchedPath}");

                matchedPath.StringToPtrToPtr(filePathPtr);

                return _nuFileDevicePC_CreateNuFileHook.OriginalFunction<IntPtr>(thisPtr, unk0, filePathPtr, unk1);
            }

            return _nuFileDeviceDat_CreateNuFileHook.OriginalFunction<IntPtr>(thisPtr, unk0, filePathPtr, unk1);
        }

        // NuFileDevicePC::CreateNuFile
        // int *__thiscall sub_57D7B0(void *this, int *a2, int a3, int a4)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr NuFileDevicePC_CreateNuFile(IntPtr thisPtr, IntPtr unk0, IntPtr filePathPtr, IntPtr unk1);

        private IntPtr NuFileDevicePC_CreateNuFileImpl(IntPtr thisPtr, IntPtr unk0, IntPtr filePathPtr, IntPtr unk1)
        {
            string filePath = filePathPtr.PtrToPtrToString();

            // Logger.Instance.Log($"{filePath}");

            return _nuFileDevicePC_CreateNuFileHook.OriginalFunction<IntPtr>(thisPtr, unk0, filePathPtr, unk1);
        }
    }
}
