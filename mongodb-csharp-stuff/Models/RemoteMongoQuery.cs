namespace Mcs.Models
{
    using MongoDB.Driver;

    public class RemoteMongoQuery
    {
        public IMongoQuery Query { get; set; }

        public IMongoSortBy SortBy { get; set; }

        public IMongoFields Fields { get; set; }

        public long? Page { get; set; }

        public long? PageSize { get; set; }
    }
}