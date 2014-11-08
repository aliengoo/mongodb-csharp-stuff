namespace Mcs.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Driver;

    using Newtonsoft.Json.Linq;

    public static class MongoDbHelper
    {
        public static MongoDatabase GetDatabase(string connectionStringName)
        {
            var connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSetting == null)
            {
                throw new ConfigurationErrorsException(string.Format("Connection {0} was not defined", connectionStringName));
            }

            var url = new MongoUrl(connectionStringSetting.ConnectionString);

            if (string.IsNullOrWhiteSpace(url.DatabaseName))
            {
                throw new ConfigurationErrorsException(string.Format("The connection {0} does not appear to have a database name", connectionStringName));
            }

            return new MongoClient(url).GetServer().GetDatabase(url.DatabaseName);
        }

        public static JArray ToJArray(this IEnumerable<BsonDocument> documents)
        {
            if (documents != null)
            {
                return new JArray(documents.Select(d => JObject.Parse(d.ToJson(new JsonWriterSettings()
                                                     {
                                                         OutputMode = JsonOutputMode.Strict
                                                     }))));
            }

            return new JArray();
        }

        public static JArray ToJArray(this MongoCursor cursor)
        {
            return cursor != null ? ((IEnumerable<BsonDocument>)cursor).ToJArray() : new JArray();
        }

        public static JObject ToJObject(this BsonDocument document)
        {
            if (document != null)
            {
                return JObject.Parse(
                    document.ToJson(new JsonWriterSettings
                                    {
                                        OutputMode = JsonOutputMode.Strict
                                    }));
            }

            return null;
        }

        public static BsonValue GetPath(this BsonValue bson, string path)
        {
            if (bson.BsonType != BsonType.Document)
            {
                throw new Exception("Not a doc");
            }

            var doc = bson.AsBsonDocument;

            var tokens = path.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
            {
                return doc;
            }

            if (!doc.Contains(tokens[0]))
            {
                return BsonNull.Value;
            }

            if (tokens.Length > 1)
            {
                return GetPath(doc[tokens[0]], tokens[1]);
            }

            return doc[tokens[0]];
        }
    }
}