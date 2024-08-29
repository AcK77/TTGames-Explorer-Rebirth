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

        private int _nuFileDeviceDatCreateNuFileOffset;
        private int _nuFileDevicePCCreateNuFileOffset;

        private Hook<NuFileDeviceDat_CreateNuFile> _nuFileDeviceDat_CreateNuFileHook;
        private Hook<NuFileDevicePC_CreateNuFile>  _nuFileDevicePC_CreateNuFileHook;

        public LMSH1Hooks(TTGamesVersion version)
        {
            _version = version;
        }

        public void Initialize()
        {
            if (_version == TTGamesVersion.LMSH1_Steamv04g)
            {
                Logger.Instance.Log("LEGOMarvel-26-09-13-Steam-v0.4g_Hotfix hooks loading...");

                _nuFileDeviceDatCreateNuFileOffset = 0x1B86A0;
                _nuFileDevicePCCreateNuFileOffset  = 0x1B6170;
            }
            else if (_version == TTGamesVersion.LMSH1_SteamCallInPatchv03b)
            {
                Logger.Instance.Log("LEGOMarvel-10-02-14-Steam-Call-In-Patch-v0.3b_Hotfix hooks loading...");

                _nuFileDeviceDatCreateNuFileOffset = 0x1BA9B0;
                _nuFileDevicePCCreateNuFileOffset  = 0x1B8480;
            }

            _modsFolderPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "Mods");
            _moddedFiles    = new List<string>(Directory.GetFiles(_modsFolderPath, "*.*", SearchOption.AllDirectories));

            _nuFileDeviceDat_CreateNuFileHook = new Hook<NuFileDeviceDat_CreateNuFile>(NuFileDeviceDat_CreateNuFileImpl, _nuFileDeviceDatCreateNuFileOffset);
            _nuFileDevicePC_CreateNuFileHook  = new Hook<NuFileDevicePC_CreateNuFile> (NuFileDevicePC_CreateNuFileImpl,  _nuFileDevicePCCreateNuFileOffset);
        }

        // NuFileDeviceDat::CreateNuFile
        // int __thiscall sub_5B86A0(void *this, int a2, int a3)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_CreateNuFile(IntPtr thisPtr, IntPtr filePathPtr, int nuFileMode);

        private int NuFileDeviceDat_CreateNuFileImpl(IntPtr thisPtr, IntPtr filePathPtr, int nuFileMode)
        {
            string filePath = filePathPtr.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            string moddedPath  = Path.Combine("Mods", filePath.Replace("/", "\\"));
            string matchedPath = _moddedFiles.Where(x => x.Contains(moddedPath)).FirstOrDefault();

            if (matchedPath != null)
            {
                Logger.Instance.Log($"Modded -> {matchedPath}");

                return _nuFileDevicePC_CreateNuFileHook.OriginalFunction<int>(matchedPath.StringToPtr(), nuFileMode);
            }

            return _nuFileDeviceDat_CreateNuFileHook.OriginalFunction<int>(thisPtr, filePathPtr, nuFileMode);
        }

        // NuFileDevicePC::CreateNuFile
        // int __stdcall sub_5B6170(LPCSTR lpFileName, int a2)

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int NuFileDevicePC_CreateNuFile(IntPtr lpFileName, int nuFileMode);

        private int NuFileDevicePC_CreateNuFileImpl(IntPtr lpFileName, int nuFileMode)
        {
            string filePath = lpFileName.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            return _nuFileDevicePC_CreateNuFileHook.OriginalFunction<int>(lpFileName, nuFileMode);
        }
    }
}
