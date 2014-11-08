namespace Mcs.Controllers
{
    using System.Web.Http;

    using Mcs.Helpers;
    using Mcs.Models;
    using Mcs.Repositories;

    public abstract class MongoQueryController : MongoController
    {
        protected MongoQueryController(IMongoRepository mongoRepository, string collectionName)
            : base(mongoRepository, collectionName)
        {
        }

        public virtual IHttpActionResult Post([FromBody]RemoteMongoQuery remoteMongoQuery)
        {
            var count = MongoRepository.Count(CollectionName, remoteMongoQuery.Query);

            var pageData = new Page
            {
                Current = remoteMongoQuery.Page.GetValueOrDefault(1),
                Size = remoteMongoQuery.PageSize.GetValueOrDefault(10),
            }.Calculate(count);

            return Ok(MongoRepository.Find(
                CollectionName,
                remoteMongoQuery.Query,
                pageData.Size,
                pageData.Skip,
                remoteMongoQuery.Fields,
                remoteMongoQuery.SortBy));
        }
    }
}