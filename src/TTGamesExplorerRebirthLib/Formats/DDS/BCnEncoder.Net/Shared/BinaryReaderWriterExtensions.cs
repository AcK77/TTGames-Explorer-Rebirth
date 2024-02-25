﻿using System.Runtime.CompilerServices;

namespace TTGamesExplorerRebirthLib.Formats.DDS.BCnEncoder.Net.Shared
{
    internal static class BinaryReaderWriterExtensions
    {
        public static unsafe void WriteStruct<T>(this BinaryWriter bw, T t) where T : unmanaged
        {
            var size = Unsafe.SizeOf<T>();
            var bytes = stackalloc byte[size];
            Unsafe.Write(bytes, t);
            var bSpan = new Span<byte>(bytes, size);
            bw.Write(bSpan);
        }

        public static unsafe T ReadStruct<T>(this BinaryReader br) where T : unmanaged
        {
            var size = Unsafe.SizeOf<T>();
            var bytes = stackalloc byte[size];
            var bSpan = new Span<byte>(bytes, size);
            br.Read(bSpan);
            return Unsafe.Read<T>(bytes);
        }

        public static void AddPadding(this BinaryWriter bw, uint padding)
        {
            for (var i = 0; i < padding; i++)
            {
                bw.Write((byte)0);
            }
        }
        public static void AddPadding(this BinaryWriter bw, int padding)
            => bw.AddPadding((uint)padding);

        public static void SkipPadding(this BinaryReader br, uint padding)
        {
            br.BaseStream.Seek(padding, SeekOrigin.Current);
        }

        public static void SkipPadding(this BinaryReader br, int padding)
            => br.SkipPadding((uint)padding);
    }
}