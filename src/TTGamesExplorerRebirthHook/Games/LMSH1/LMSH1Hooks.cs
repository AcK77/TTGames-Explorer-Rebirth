using System;
using System.Runtime.InteropServices;
using TTGamesExplorerRebirthHook.Mod;
using TTGamesExplorerRebirthHook.Utils;

namespace TTGamesExplorerRebirthHook.Games.LMSH1
{
    public class LMSH1Hooks : ITTGames
    {
        private readonly TTGamesVersion _version;

        private int _nuFileDeviceDat_CreateNuFileOffset;
        private int _nuFileDeviceDat_FileSizeOffset;
        private int _nuFileDeviceDat_GetPositionOnDiscOffset;
        private int _nuFileDevicePC_CreateNuFileOffset;

        private Hook<NuFileDeviceDat_CreateNuFile>      _nuFileDeviceDat_CreateNuFileHook;
        private Hook<NuFileDeviceDat_FileSize>          _nuFileDeviceDat_FileSizeHook;
        private Hook<NuFileDeviceDat_GetPositionOnDisc> _nuFileDeviceDat_GetPositionOnDiscHook;
        private Hook<NuFileDevicePC_CreateNuFile>       _nuFileDevicePC_CreateNuFileHook;

        public LMSH1Hooks(TTGamesVersion version)
        {
            _version = version;
        }

        public void Initialize()
        {
            if (_version == TTGamesVersion.LMSH1_Steamv04g)
            {
                Logger.Instance.Log("LEGOMarvel-26-09-13-Steam-v0.4g_Hotfix hooks loading...");

                _nuFileDeviceDat_CreateNuFileOffset      = 0x1B86A0;
                _nuFileDeviceDat_FileSizeOffset          = 0x1B0A70;
                _nuFileDeviceDat_GetPositionOnDiscOffset = 0x1B0830;
                _nuFileDevicePC_CreateNuFileOffset       = 0x1B6170;

            }
            else if (_version == TTGamesVersion.LMSH1_SteamCallInPatchv03b)
            {
                Logger.Instance.Log("LEGOMarvel-10-02-14-Steam-Call-In-Patch-v0.3b_Hotfix hooks loading...");

                _nuFileDeviceDat_CreateNuFileOffset      = 0x1BA9B0;
                _nuFileDeviceDat_FileSizeOffset          = 0x1B2D70;
                _nuFileDeviceDat_GetPositionOnDiscOffset = 0x1B2B30;
                _nuFileDevicePC_CreateNuFileOffset       = 0x1B8480;
            }

            _nuFileDeviceDat_CreateNuFileHook      = new Hook<NuFileDeviceDat_CreateNuFile>     (NuFileDeviceDat_CreateNuFileImpl,      _nuFileDeviceDat_CreateNuFileOffset);
            _nuFileDeviceDat_FileSizeHook          = new Hook<NuFileDeviceDat_FileSize>         (NuFileDeviceDat_FileSizeImpl,          _nuFileDeviceDat_FileSizeOffset);
            _nuFileDeviceDat_GetPositionOnDiscHook = new Hook<NuFileDeviceDat_GetPositionOnDisc>(NuFileDeviceDat_GetPositionOnDiscImpl, _nuFileDeviceDat_GetPositionOnDiscOffset);
            _nuFileDevicePC_CreateNuFileHook       = new Hook<NuFileDevicePC_CreateNuFile>      (NuFileDevicePC_CreateNuFileImpl,       _nuFileDevicePC_CreateNuFileOffset);
        }

        // NuFileDeviceDat::CreateNuFile
        // int __thiscall sub_5B86A0(void *this, int a2, int a3)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_CreateNuFile(IntPtr thisPtr, IntPtr filePathPtr, int nuFileMode);

        private int NuFileDeviceDat_CreateNuFileImpl(IntPtr thisPtr, IntPtr filePathPtr, int nuFileMode)
        {
            string filePath = filePathPtr.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            if (ModFolder.IsFileModded(filePath))
            {
                ModFile moddedFile = ModFolder.GetModdedFile(filePath);

                Logger.Instance.Log($"Modded -> {moddedFile.Path}");

                return _nuFileDevicePC_CreateNuFileHook.OriginalFunction<int>(moddedFile.Path.StringToPtr(), nuFileMode);
            }

            return _nuFileDeviceDat_CreateNuFileHook.OriginalFunction<int>(thisPtr, filePathPtr, nuFileMode);
        }

        // NuFileDeviceDat::FileSize
        // int __thiscall sub_5B0A70(void *this, int filePath)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int NuFileDeviceDat_FileSize(IntPtr thisPtr, IntPtr filePathPtr);

        public int NuFileDeviceDat_FileSizeImpl(IntPtr thisPtr, IntPtr filePathPtr)
        {
            string filePath = filePathPtr.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            if (ModFolder.IsFileModded(filePath))
            {
                ModFile moddedFile = ModFolder.GetModdedFile(filePath);

                Logger.Instance.Log($"Modded -> {moddedFile.Path} ({moddedFile.Size} bytes)");

                return moddedFile.Size;
            }

            return _nuFileDeviceDat_FileSizeHook.OriginalFunction<int>(thisPtr, filePathPtr);
        }

        // NuFileDeviceDat::GetPositionOnDisc
        // char __thiscall sub_5B0830(int *this, int a2, _QWORD *a3)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate byte NuFileDeviceDat_GetPositionOnDisc(IntPtr thisPtr, IntPtr filePathPtr, IntPtr position);

        public byte NuFileDeviceDat_GetPositionOnDiscImpl(IntPtr thisPtr, IntPtr filePathPtr, IntPtr position)
        {
            string filePath = filePathPtr.PtrToString();

            // Logger.Instance.Log($"{filePath}");

            if (ModFolder.IsFileModded(filePath))
            {
                Logger.Instance.Log($"Modded -> {ModFolder.GetModdedFile(filePath).Path}");

                // NOTE: Game seems to doesn't care about the position since we layered it.

                return 1;
            }

            return _nuFileDeviceDat_GetPositionOnDiscHook.OriginalFunction<byte>(thisPtr, filePathPtr, position); ;
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
