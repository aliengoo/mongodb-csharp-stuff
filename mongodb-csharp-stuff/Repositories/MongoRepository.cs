namespace Mcs.Repositories
{
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class MongoRepository : IMongoRepository
    {
        public MongoDatabase Database { get; private set; }

        public MongoRepository(MongoDatabase database)
        {
            Database = database;
        }

        public MongoCollection this[string collectionName]
        {
            get
            {
                return GetCollection(collectionName);    
            }
        }

        public MongoCollection GetCollection(string name)
        {
            return Database.GetCollection(name);
        }

        public MongoCollection<T> GetCollection<T>(string name)
        {
            return Database.GetCollection<T>(name);
        }

        public long Count(string collectionName, IMongoQuery query)
        {
            return Database.GetCollection(collectionName).Count(query);
        }

        public MongoCursor Find(
            string collectionName,
            IMongoQuery query,
            long? limit,
            long? skip,
            IMongoFields fields,
            IMongoSortBy sortBy)
        {
            var c = GetExistingCollection(collectionName);

            var q = c.Find(query);

            if (limit.HasValue)
            {
                q.SetLimit((int)limit.Value);
            }

            if (skip.HasValue)
            {
                q.SetSkip((int)skip.Value);
            }

            if (fields != null)
            {
                q.SetFields(fields);
            }

            if (sortBy != null)
            {
                q.SetSortOrder(sortBy);
            }

            return q;
        }

        public BsonDocument FindOne(string collectionName, IMongoQuery query)
        {
            var c = GetExistingCollection(collectionName);

            return c.FindOne(query);
        }

        public MongoCursor FindAll(string collectionName, IMongoFields fields, IMongoSortBy sortBy)
        {
            var c = GetExistingCollection(collectionName);

            var q = c.FindAll();

            if (fields != null)
            {
                q.SetFields(fields);
            }

            if (sortBy != null)
            {
                q.SetSortOrder(sortBy);
            }

            return q;
        }

        public BsonDocument Save(string collectionName, BsonDocument doc)
        {
            var c = GetExistingCollection(collectionName);

            c.Save(doc);

            return doc;
        }

        public BsonDocument FindById(string collectionName, BsonValue id)
        {
            var c = GetExistingCollection(collectionName);

            return c.FindOneById(id);
        }

        public WriteConcernResult Remove(string collectionName, IMongoQuery query)
        {
            var c = GetExistingCollection(collectionName);

            return c.Remove(query);
        }

        private MongoCollection<BsonDocument> GetExistingCollection(string collectionName)
        {
            if (!Database.CollectionExists(collectionName))
            {
                throw new MongoException(string.Format("Collection {0} does not exist", collectionName));
            }

            return Database.GetCollection(collectionName);
        }
    }
}