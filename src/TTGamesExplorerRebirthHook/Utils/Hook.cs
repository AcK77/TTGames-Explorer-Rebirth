using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TTGamesExplorerRebirthHook.Utils
{
    public unsafe class Hook<TDelegate> where TDelegate : Delegate
    {
        private readonly byte[] _originalBytes;
        private byte[]          _jmpBytes;

        private readonly IntPtr    _targetAddress;
        private readonly IntPtr    _hookAddress;
        private readonly TDelegate _hookDelegate;

        public static IntPtr BaseAddress => Process.GetCurrentProcess().MainModule.BaseAddress;

        public Hook(TDelegate dlg, int address)
        {
            _hookDelegate  = dlg;
            _targetAddress = new IntPtr(BaseAddress.ToInt32() + address);
            _hookAddress   = Marshal.GetFunctionPointerForDelegate(_hookDelegate);

            CreateJMPInstructions();

            _originalBytes = new byte[_jmpBytes.Length];

            fixed (byte* p = _originalBytes)
            {
                ProtectionSafeMemoryCopy(new IntPtr(p), _targetAddress, _originalBytes.Length);
            }

            Enable();

            Logger.Instance.Log($"{_hookDelegate.Method.Name} hook initialized.");
        }

        public T OriginalFunction<T>(params object[] args)
        {
            Disable();

            object dlgResult = Marshal.GetDelegateForFunctionPointer<TDelegate>(_targetAddress).DynamicInvoke(args);

            Enable();

            return (T)dlgResult;
        }

        public void OriginalFunction(params object[] args)
        {
            Disable();

            Marshal.GetDelegateForFunctionPointer<TDelegate>(_targetAddress).DynamicInvoke(args);

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

            if (!Natives.VirtualProtect(destination, bufferSize, Natives.VirtualProtectionType.ExecuteReadWrite, out Natives.VirtualProtectionType oldProtection))
            {
                throw new Exception($"Failed to unprotect memory. OldProtection: {oldProtection}");
            }

            byte* pointerDestination = (byte*)destination;
            byte* pointerSource      = (byte*)source;

            for (int i = 0; i < count; i++)
            {
                *(pointerDestination + i) = *(pointerSource + i);
            }

            if (!Natives.VirtualProtect(destination, bufferSize, oldProtection, out oldProtection))
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