namespace Mcs.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Driver;

    using Newtonsoft.Json.Linq;

    public static class BsonDocumentHelpers
    {
        public static ExpandoObject ToExpando(this BsonDocument document)
        {
            return document.ToDictionary().ToExpando();
        }

        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
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

        public static JArray ToJArray(this IEnumerable<BsonDocument> documents)
        {
            if (documents != null)
            {
                return
                    new JArray(
                        documents.Select(
                            d =>
                                JObject.Parse(d.ToJson(new JsonWriterSettings() { OutputMode = JsonOutputMode.Strict }))));
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
                return JObject.Parse(document.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }));
            }

            return null;
        }
    }
}