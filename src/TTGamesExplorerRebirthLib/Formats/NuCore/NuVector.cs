using TTGamesExplorerRebirthLib.Helper;

namespace TTGamesExplorerRebirthLib.Formats.NuCore
{
#pragma warning disable IDE0059
    public class NuVector
    {
        public const string Magic = "ROTV";

        public static T Deserialize<T>(BinaryReader reader, NuResourceHeader nuResourceHeader)
        {
            if (reader.ReadUInt32AsString() != Magic)
            {
                throw new InvalidDataException($"{reader.BaseStream.Position:x8}");
            }

            uint size = reader.ReadUInt32BigEndian();

            if (typeof(T) == typeof(NuResourceReference))
            {
                uint id = reader.ReadUInt32BigEndian();

                return (T)(object)new NuResourceReference().Deserialize(reader, nuResourceHeader, size);
            }
            else if (typeof(T) == typeof(NuVFXLocator))
            {
                return (T)(object)new NuVFXLocator().Deserialize();
            }
            else if (typeof(T) == typeof(NuSpline[]))
            {
                NuSpline[] nuSplines = new NuSpline[size];

                for (int i = 0; i < size; i++)
                {
                    nuSplines[i] = new NuSpline().Deserialize();
                }

                return (T)(object)nuSplines;
            }
            else if (typeof(T) == typeof(ushort[]))
            {
                ushort[] ushortArray = new ushort[size];

                for (int i = 0; i < size; i++)
                {
                    ushortArray[i] = reader.ReadUInt16();
                }

                return (T)(object)ushortArray;
            }
            else
            {
                throw new NotSupportedException($"{reader.BaseStream.Position:x8}");
            }
        }
    }
#pragma warning restore IDE0059
}
