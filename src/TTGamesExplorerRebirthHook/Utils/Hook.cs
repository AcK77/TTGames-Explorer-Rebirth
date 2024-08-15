using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TTGamesExplorerRebirthHook.Utils
{
    public unsafe class Hook
    {
        private byte[] _originalBytes;
        private byte[] _jmpBytes;

        private IntPtr _targetAddress;
        private IntPtr _hookAddress;

        public static IntPtr BaseAddress => Process.GetCurrentProcess().MainModule.BaseAddress;

        public enum VirtualProtectionType : uint
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            Readonly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400,
        }

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, VirtualProtectionType flNewProtect, out VirtualProtectionType lpflOldProtect);

        public Hook() { }

        public TDelegate CreateHook<TDelegate>(TDelegate dlg, int address) where TDelegate : Delegate
        {
            _targetAddress = new IntPtr(BaseAddress.ToInt32() + address);
            _hookAddress = Marshal.GetFunctionPointerForDelegate(dlg);

            CreateJMPInstructions();

            _originalBytes = new byte[_jmpBytes.Length];

            fixed (byte* p = _originalBytes)
            {
                ProtectionSafeMemoryCopy(new IntPtr(p), _targetAddress, _originalBytes.Length);
            }

            Enable();

            Logger.Instance.Log($"{dlg.Method.Name} hook initialized.");

            return dlg;
        }

        public T OriginalFunction<T, TDelegate>(params object[] args) where TDelegate : Delegate
        {
            TDelegate dlg = Marshal.GetDelegateForFunctionPointer<TDelegate>(_targetAddress);

            Disable();

            object dlgResult = dlg.DynamicInvoke(args);

            Enable();

            return (T)dlgResult;
        }

        public void OriginalFunction<TDelegate>(params object[] args) where TDelegate : Delegate
        {
            TDelegate dlg = Marshal.GetDelegateForFunctionPointer<TDelegate>(_targetAddress);

            Disable();

            dlg.DynamicInvoke(args);

            Enable();
        }

        private void Enable()
        {
            fixed (byte* p = _jmpBytes)
            {
                ProtectionSafeMemoryCopy(_targetAddress, new IntPtr(p), _jmpBytes.Length);
            }
        }

        private void Disable()
        {
            fixed (byte* p = _originalBytes)
            {
                ProtectionSafeMemoryCopy(_targetAddress, new IntPtr(p), _originalBytes.Length);
            }
        }

        public static void ProtectionSafeMemoryCopy(IntPtr destination, IntPtr source, int count)
        {
            UIntPtr bufferSize = new UIntPtr((uint)count);

            if (!VirtualProtect(destination, bufferSize, VirtualProtectionType.ExecuteReadWrite, out VirtualProtectionType oldProtection))
            {
                throw new Exception($"Failed to unprotect memory. OldProtection: {oldProtection}");
            }

            byte* pointerDestination = (byte*)destination;
            byte* pointerSource = (byte*)source;

            for (int i = 0; i < count; i++)
            {
                *(pointerDestination + i) = *(pointerSource + i);
            }

            if (!VirtualProtect(destination, bufferSize, oldProtection, out oldProtection))
            {
                throw new Exception($"Failed to unprotect memory. OldProtection: {oldProtection}");
            }
        }

        private void CreateJMPInstructions()
        {
            List<byte> bytesList = new List<byte>
            {
                0xb8 // mov
            };

            bytesList.AddRange(BitConverter.GetBytes(_hookAddress.ToInt32())); // func addr

            bytesList.Add(0xff); // jmp
            bytesList.Add(0xe0);

            _jmpBytes = bytesList.ToArray();
        }
    }
}