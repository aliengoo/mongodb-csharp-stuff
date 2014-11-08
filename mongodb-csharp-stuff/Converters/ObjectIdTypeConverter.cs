namespace Mcs.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using MongoDB.Bson;

    public class ObjectIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(int);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {

            var s = value as string;

            if (s != null)
            {
                return ObjectId.Parse(s);
            }

            throw new Exception("Unable to parse object id");
        }
    }
}