using System.Reflection;

namespace DXDecompiler.Chunks
{
    public static class EnumExtensions
    {
        private static readonly Dictionary<Type, Dictionary<Type, Dictionary<Enum, Attribute[]>>> AttributeValues;

        static EnumExtensions()
        {
            AttributeValues = [];
        }

        public static string GetDescription(this Enum value, ChunkType chunkType = ChunkType.Unknown)
        {
            return value.GetAttributeValue<DescriptionAttribute, string>((a, v) =>
            {
                var attribute = a.FirstOrDefault(x => x.ChunkType == chunkType);
                if (attribute == null)
                    return v.ToString();
                return attribute.Description;
            });
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Enum attributeValue,
            Func<TAttribute[], Enum, TValue> getValueCallback)
            where TAttribute : Attribute
        {
            Type type = attributeValue.GetType();

            if (!AttributeValues.ContainsKey(type))
                AttributeValues[type] = [];

            var attributeValuesForType = AttributeValues[type];

            var attributeType = typeof(TAttribute);
            if (!attributeValuesForType.ContainsKey(attributeType))
                attributeValuesForType[attributeType] = Enum.GetValues(type).Cast<Enum>().Distinct()
                    .ToDictionary(x => x, GetAttribute<TAttribute>);

            var attributeValues = attributeValuesForType[attributeType];
            if (!attributeValues.TryGetValue(attributeValue, out Attribute[] value))
                throw new ArgumentException(string.Format("Could not find attribute value for type '{0}' and value '{1}'.", type, attributeValue));
            return getValueCallback((TAttribute[])value, attributeValue);
        }

        private static Attribute[] GetAttribute<TAttribute>(Enum value)
            where TAttribute : Attribute
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            return Attribute.GetCustomAttributes(field, typeof(TAttribute));
        }
    }
}