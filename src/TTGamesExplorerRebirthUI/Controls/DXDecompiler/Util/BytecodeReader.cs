﻿using System.Text;

namespace DXDecompiler.Util
{
    public class BytecodeReader(byte[] buffer, int index, int count)
    {
        private readonly byte[] _buffer = buffer;
        private readonly int _offset = index;
        private readonly BinaryReader _reader = new(new MemoryStream(buffer, index, count));

        public bool EndOfBuffer
        {
            get { return _reader.BaseStream.Position >= _reader.BaseStream.Length; }
        }

        public long CurrentPosition
        {
            get { return _reader.BaseStream.Position; }
        }

        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

        public float ReadSingle()
        {
            return _reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        public ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }

        public string ReadString()
        {
            var sb = new StringBuilder();
            char nextCharacter;
            while (!EndOfBuffer && (nextCharacter = _reader.ReadChar()) != 0)
            {
                sb.Append(nextCharacter);
            }

            return sb.ToString();
        }

        public BytecodeReader CopyAtCurrentPosition(int? count = null)
        {
            return CopyAtOffset((int)_reader.BaseStream.Position, count);
        }

        public BytecodeReader CopyAtOffset(int offset, int? count = null)
        {
            count ??= (int)(_reader.BaseStream.Length - offset);

            return new BytecodeReader(_buffer, _offset + offset, count.Value);
        }
    }
}