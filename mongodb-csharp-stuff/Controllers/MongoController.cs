namespace Mcs.Controllers
{
    using System.Web.Http;

    using Mcs.Repositories;

    public abstract class MongoController : ApiController
    {
        private readonly IMongoRepository _mongoRepository;

        private readonly string _collectionName;

        protected MongoController(IMongoRepository mongoRepository, string collectionName)
        {
            _mongoRepository = mongoRepository;
            _collectionName = collectionName;
        }

        protected virtual IMongoRepository MongoRepository
        {
            get
            {
                return _mongoRepository;
            }
        }

        protected virtual string CollectionName
        {
            get
            {
                return _collectionName;
            }
        }
    }
}