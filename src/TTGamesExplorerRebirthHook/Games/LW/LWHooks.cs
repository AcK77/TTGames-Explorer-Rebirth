using System;
using System.Runtime.InteropServices;
using TTGamesExplorerRebirthHook.Mod;
using TTGamesExplorerRebirthHook.Utils;

namespace TTGamesExplorerRebirthHook.Games.LW
{
    public class LWHooks : ITTGames
    {
        private readonly TTGamesVersion _version;

        private int _nuFileDeviceDat_CreateFileOffset;
        private int _nuFileDeviceDat_FileSizeOffset;
        private int _nuFileDeviceDat_FileGetPositionOffset;
        private int _nuFileDevicePC_CreateFileOffset;

        private Hook<NuFileDeviceDat_CreateFile>      _nuFileDeviceDat_CreateFileHook;
        private Hook<NuFileDeviceDat_FileSize>        _nuFileDeviceDat_FileSizeHook;
        private Hook<NuFileDeviceDat_FileGetPosition> _nuFileDeviceDat_FileGetPositionHook;
        private Hook<NuFileDevicePC_CreateFile>       _nuFileDevicePC_CreateFileHook;

        public LWHooks(TTGamesVersion version)
        {
            _version = version;
        }

        public void Initialize()
        {
            if (_version == TTGamesVersion.LW_TU3Dv01)
            {
                Logger.Instance.Log("BADGER-07-12-18-TU3D-v0.1_Hotfix hooks loading...");

                _nuFileDeviceDat_CreateFileOffset      = 0x17D5D0;
                _nuFileDeviceDat_FileSizeOffset        = 0x180220;
                _nuFileDeviceDat_FileGetPositionOffset = 0x17FF50;
                _nuFileDevicePC_CreateFileOffset       = 0x17D7B0;
            }

            _nuFileDeviceDat_CreateFileHook      = new Hook<NuFileDeviceDat_CreateFile>     (NuFileDeviceDat_CreateFileImpl,      _nuFileDeviceDat_CreateFileOffset);
            _nuFileDeviceDat_FileSizeHook        = new Hook<NuFileDeviceDat_FileSize>       (NuFileDeviceDat_FileSizeImpl,        _nuFileDeviceDat_FileSizeOffset);
            _nuFileDeviceDat_FileGetPositionHook = new Hook<NuFileDeviceDat_FileGetPosition>(NuFileDeviceDat_FileGetPositionImpl, _nuFileDeviceDat_FileGetPositionOffset);
            _nuFileDevicePC_CreateFileHook       = new Hook<NuFileDevicePC_CreateFile>      (NuFileDevicePC_CreateFileImpl,       _nuFileDevicePC_CreateFileOffset);
        }

        // NuFileDeviceDat::CreateNuFile
        // _DWORD *__thiscall sub_57D5D0(char *this, int a2, int a3, int a4)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr NuFileDeviceDat_CreateFile(IntPtr thisPtr, IntPtr unk0, IntPtr filePathPtr, IntPtr unk1);

        private IntPtr NuFileDeviceDat_CreateFileImpl(IntPtr thisPtr, IntPtr unk0, IntPtr filePathPtr, IntPtr unk1)
        {
            string filePath = filePathPtr.PtrToPtrToString();

            // Logger.Instance.Log($"{filePath}");

            if (ModFolder.IsFileModded(filePath))
            {
                ModFile moddedFile = ModFolder.GetModdedFile(filePath);

                Logger.Instance.Log($"Modded -> {moddedFile.Path}");

                moddedFile.Path.StringToPtrToPtr(filePathPtr);

                return _nuFileDevicePC_CreateFileHook.OriginalFunction<IntPtr>(thisPtr, unk0, filePathPtr, unk1);
            }

            return _nuFileDeviceDat_CreateFileHook.OriginalFunction<IntPtr>(thisPtr, unk0, filePathPtr, unk1);
        }

        // NuFileDeviceDat::FileSize
        // __int64 __thiscall sub_580220(void *this, char **a2)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate long NuFileDeviceDat_FileSize(IntPtr thisPtr, IntPtr filePathPtr);

        public long NuFileDeviceDat_FileSizeImpl(IntPtr thisPtr, IntPtr filePathPtr)
        {
            string filePath = filePathPtr.PtrToPtrToString();

            // Logger.Instance.Log($"{filePath}");

            if (ModFolder.IsFileModded(filePath))
            {
                ModFile moddedFile = ModFolder.GetModdedFile(filePath);

                Logger.Instance.Log($"Modded -> {moddedFile.Path} ({moddedFile.Size} bytes)");

                return moddedFile.Size;
            }

            return _nuFileDeviceDat_FileSizeHook.OriginalFunction<long>(thisPtr, filePathPtr);
        }

        // NuFileDeviceDat::FileGetPosition
        // char __thiscall sub_57FF50(int *this, const char **a2, _QWORD *a3)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate byte NuFileDeviceDat_FileGetPosition(IntPtr thisPtr, IntPtr filePathPtr, IntPtr position);

        public byte NuFileDeviceDat_FileGetPositionImpl(IntPtr thisPtr, IntPtr filePathPtr, IntPtr position)
        {
            string filePath = filePathPtr.PtrToPtrToString();

            // Logger.Instance.Log($"{filePath}");

            if (ModFolder.IsFileModded(filePath))
            {
                Logger.Instance.Log($"Modded -> {ModFolder.GetModdedFile(filePath).Path}");

                // NOTE: Game seems to doesn't care about the position since we layered it.

                return 1;
            }

            return _nuFileDeviceDat_FileGetPositionHook.OriginalFunction<byte>(thisPtr, filePathPtr, position);
        }

        // NuFileDevicePC::CreateFile
        // int *__thiscall sub_57D7B0(void *this, int *a2, int a3, int a4)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr NuFileDevicePC_CreateFile(IntPtr thisPtr, IntPtr unk0, IntPtr filePathPtr, IntPtr unk1);

        private IntPtr NuFileDevicePC_CreateFileImpl(IntPtr thisPtr, IntPtr unk0, IntPtr filePathPtr, IntPtr unk1)
        {
            string filePath = filePathPtr.PtrToPtrToString();

            // Logger.Instance.Log($"{filePath}");

            return _nuFileDevicePC_CreateFileHook.OriginalFunction<IntPtr>(thisPtr, unk0, filePathPtr, unk1);
        }
    }
}
