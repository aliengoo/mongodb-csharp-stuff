namespace Mcs.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class CryptoDoc
    {
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.Document)]
        public Dictionary<string, object> Metadata { get; set; }

        public List<string> Tags { get; set; }

        public byte[] Data { get; set; }
    }
}