namespace Mcs.Repositories
{
    using System.Collections.Generic;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;

    using Mcs.Models;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class CryptoMongoRepository
    {
        private readonly IMongoRepository _mongoRepository;

        private readonly string _collectionName;

        private readonly string _rsaKey;

        public CryptoMongoRepository(IMongoRepository mongoRepository, string collectionName, string rsaKey)
        {
            _mongoRepository = mongoRepository;
            _collectionName = collectionName;

            if (string.IsNullOrWhiteSpace(rsaKey))
            {
                throw new SecurityException("rsaKey is required");
            }
            _rsaKey = rsaKey;
        }

        public virtual CryptoDoc Save(BsonDocument docToEncrypt, CryptoDoc cd)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                rsa.FromXmlString(_rsaKey);

                var plainText = docToEncrypt.ToJson();

                cd.Data = rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), false);

                _mongoRepository.GetCollection<CryptoDoc>(_collectionName).Save(cd);

                return cd;
            }
        }

        public virtual CryptoDoc Find(ObjectId id)
        {
            return _mongoRepository.GetCollection<CryptoDoc>(_collectionName).FindOneById(id);
        }

        public virtual IEnumerable<CryptoDoc> Find(
            IMongoQuery query,
            IMongoFields fields,
            IMongoSortBy sortyBy,
            int? limit,
            int? skip)
        {
            var find = _mongoRepository.GetCollection<CryptoDoc>(_collectionName).Find(query);

            if (fields != null)
            {
                find.SetFields(fields);
            }

            if (sortyBy != null)
            {
                find.SetSortOrder(sortyBy);
            }

            if (limit.HasValue)
            {
                find.SetLimit(limit.Value);
            }

            if (skip.HasValue)
            {
                find.SetSkip(skip.Value);
            }

            return find;
        }

        public virtual void Remove(ObjectId id)
        {
            _mongoRepository.Remove(_collectionName, Query<CryptoDoc>.EQ(cd => cd.Id, id));
        }
    }
}