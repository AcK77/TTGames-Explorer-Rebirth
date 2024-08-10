using System.Collections;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats
{
    /// <summary>
    /// https://drive.google.com/drive/folders/1aOr9YKbZfBkaYvkpQPEY1Rk0s6YUZSzU
    /// </summary>
    public class An3
    {
        private const string MagicAni4 = "4INA";

        public string Name = "";

        public struct SkeletonMatrix
        {
            public ushort TranslationX;
            public ushort TranslationY;
            public ushort TranslationZ;

            public ushort RotationX;
            public ushort RotationY;
            public ushort RotationZ;

            public ushort ScaleX;
            public ushort ScaleY;
            public ushort ScaleZ;
        }

        [Flags]
        public enum UnknownBonesFlags : byte
        {
            Nothing  = 0,
            Unknown1 = 0b00000001,
            Unknown2 = 0b00000010,
            Unknown3 = 0b00000100,
            Unknown4 = 0b00001000,
            Unknown5 = 0b00010000,
            Unknown6 = 0b00100000,
            Unknown7 = 0b01000000,
            Unknown8 = 0b10000000,
        }

        public An3(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }

        public An3(byte[] buffer)
        {
            Deserialize(buffer);
        }

        private void Deserialize(byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using BinaryReader reader = new(stream);

            // Read header.

            if (reader.ReadUInt32AsString() != MagicAni4)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            ushort bonesCounter = reader.ReadUInt16();
            ushort unknown1 = reader.ReadUInt16();
            byte framesCounter = reader.ReadByte(); // Size?
            byte unknown3 = reader.ReadByte(); // Counter?
            byte unknown4 = reader.ReadByte(); // Counter?
            byte unknown5 = reader.ReadByte(); // Counter?
            uint unknown6 = reader.ReadUInt32(); // Always 9?
            byte[] padding1 = reader.ReadBytes(0xC); // Padding?
            int hash1 = reader.ReadInt32();
            int hash2 = reader.ReadInt32();

            uint movementHeadOffset = reader.ReadUInt32();
            uint staticDataOffset = reader.ReadUInt32();
            uint skeletonMatrixOffset = reader.ReadUInt32(); // SkeletonMatrix size 0x18 - [x-pos][y-pos][z-pos][x-rot][y-rot][z-rot][x-size][y-size][z-size]
            uint movementDataOffset = reader.ReadUInt32();
            uint bonesFlagsOffset = reader.ReadUInt32();
            uint padding2 = reader.ReadUInt32(); // Always 0?
            byte[] animName = reader.ReadBytes(0x20);

            Name = Encoding.ASCII.GetString(animName).TrimEnd('\0');

            // Read file.

            reader.BaseStream.Seek(staticDataOffset, SeekOrigin.Begin);

            short[] staticData = new short[(skeletonMatrixOffset - staticDataOffset) / 2];

            for (int i = 0; i < staticData.Length; i++)
            {
                staticData[i] = reader.ReadInt16();
            }

            if (reader.BaseStream.Position != skeletonMatrixOffset)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            List<SkeletonMatrix> skeletonMatrices = [];

            for (int i = 0; i < bonesCounter; i++)
            {
                // NOTE: Values are index of staticData - 0x10; Seems to be bytes insteads of ushorts. 0x06 means the bone moves.
                skeletonMatrices.Add(new SkeletonMatrix()
                {
                    TranslationX = reader.ReadUInt16(),
                    TranslationY = reader.ReadUInt16(),
                    TranslationZ = reader.ReadUInt16(),

                    RotationX = reader.ReadUInt16(),
                    RotationY = reader.ReadUInt16(),
                    RotationZ = reader.ReadUInt16(),

                    ScaleX = reader.ReadUInt16(),
                    ScaleY = reader.ReadUInt16(),
                    ScaleZ = reader.ReadUInt16(),
                });
            }

            // NOTE: The extra padding here is really strange.

            reader.BaseStream.Seek(movementHeadOffset, SeekOrigin.Begin);

            float[] movementHeadValues = new float[framesCounter];

            for (int i = 0; i < framesCounter; i++)
            {
                movementHeadValues[i] = (float)BitConverter.Int16BitsToHalf(reader.ReadInt16());
            }

            if (reader.BaseStream.Position != movementDataOffset)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            int[] movementMatrixValues = new int[framesCounter];

            for (int i = 0; i < framesCounter; i++)
            {
                movementMatrixValues[i] = reader.ReadInt32();
            }

            // NOTE: The extra padding here is really strange. Sometimes it's equal to "framesCounter" bytes.

            reader.BaseStream.Seek(bonesFlagsOffset, SeekOrigin.Begin);

            List<UnknownBonesFlags> bonesFlags = [];

            for (int i = 0; i < bonesCounter; i++)
            {
                bonesFlags.Add((UnknownBonesFlags)reader.ReadByte());
            }

            uint finalSize = (uint)(reader.BaseStream.Position + 3 & ~0x03);

            if (reader.BaseStream.Length != finalSize)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }
        }
    }
}