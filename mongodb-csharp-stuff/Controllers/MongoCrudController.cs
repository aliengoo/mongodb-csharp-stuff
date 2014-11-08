namespace Mcs.Controllers
{
    using System.Web.Http;
    using System.Web.Http.ModelBinding;

    using Mcs.Repositories;

    using MongoDB.Bson;

    /// <summary>
    /// Forced to override, simply so you decide how authorization should work.
    /// </summary>
    public abstract class MongoCrudController : MongoController
    {
        protected MongoCrudController(IMongoRepository mongoRepository, string collectionName) : base (mongoRepository, collectionName)
        {
        }

        public virtual IHttpActionResult Get([ModelBinder]ObjectId id)
        {
            return Ok(MongoRepository.FindById(CollectionName, id));
        }

        public virtual IHttpActionResult Post([FromBody]BsonDocument document)
        {
            return Ok(MongoRepository.Save(CollectionName, document));
        }

        public virtual IHttpActionResult Put([ModelBinder]ObjectId id, [FromBody]BsonDocument document)
        {
            return Ok(MongoRepository.Save(CollectionName, document));
        }

        public virtual IHttpActionResult Delete([FromUri][ModelBinder]ObjectId id)
        {
            return Ok(MongoRepository.Remove(CollectionName, MongoDB.Driver.Builders.Query.EQ("_id", id)));
        }
    }
}