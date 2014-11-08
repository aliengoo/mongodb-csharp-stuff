namespace Mcs.Converters
{
    using System;

    using Mcs.Models;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class RemoteMongoQueryConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (CanConvert(objectType))
            {
                var json = JObject.Load(reader);

                JToken token;

                var remoteMongoQuery = new RemoteMongoQuery();

                if (json.TryGetValue("query", out token))
                {
                    remoteMongoQuery.Query = new QueryDocument(BsonDocument.Parse(token.ToString()));
                }

                if (json.TryGetValue("sortBy", out token))
                {
                    remoteMongoQuery.SortBy = new SortByDocument(BsonDocument.Parse(token.ToString()));
                }

                if (json.TryGetValue("fields", out token))
                {
                    remoteMongoQuery.Fields = new FieldsDocument(BsonDocument.Parse(token.ToString()));
                }

                if (json.TryGetValue("page", out token))
                {
                    remoteMongoQuery.Page = token.Value<long>();
                }

                if (json.TryGetValue("pageSize", out token))
                {
                    remoteMongoQuery.PageSize = token.Value<long>();
                }

                return remoteMongoQuery;
            }

            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RemoteMongoQuery);
        }
    }
}