namespace Mcs.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using JsonReader = Newtonsoft.Json.JsonReader;
    using JsonWriter = Newtonsoft.Json.JsonWriter;

    public class BsonDocumentJsonConverter : JsonConverter
    {
        public override void WriteJson(
            JsonWriter writer, object value, JsonSerializer serializer)
        {
            var settings = new JsonWriterSettings
            {
                OutputMode = JsonOutputMode.Strict
            };


            var cursor = value as IEnumerable<BsonDocument>;

            if (cursor == null)
            {
                var document = value as BsonDocument;

                if (document != null)
                {
                    writer.WriteRawValue(document.ToJson(settings));
                }
            }
            else
            {
                writer.WriteStartArray();

                foreach (BsonDocument document in cursor)
                {
                    writer.WriteRawValue(document.ToJson(settings));
                }

                writer.WriteEndArray();
            }
        }

        public override object ReadJson(
            JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(BsonDocument))
            {
                return BsonDocument.Parse(JToken.ReadFrom(reader).ToString());
            }

            var list = new List<BsonDocument>();

            if (objectType == typeof(IEnumerable<BsonDocument>))
            {
                var arr = JToken.ReadFrom(reader);

                // this was not an array
                if (arr.Type != JTokenType.Array)
                {
                    throw new Exception(string.Format("Expected JSON array, but got a {0}", arr.Type));
                }

                list.AddRange(from item in (JArray)arr select BsonDocument.Parse(item.ToString()));

                return list;
            }

            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            var types = new[] { typeof(IEnumerable<BsonDocument>), typeof(BsonDocument) };
            return types.Any(t => t == objectType);
        }
    }
}