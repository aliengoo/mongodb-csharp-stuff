namespace Mcs.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Mcs.Repositories;

    using Microsoft.Owin;

    public class MongoCollectionRightsMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public MongoCollectionRightsMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);

            // TODO : Get the current principal here
            var user = context.Request.User;

            if (user == null)
            {

            }

            var mongoIndex = Array.FindIndex(context.Request.Uri.Segments, s => s == "mongo/");

            if (mongoIndex != -1)
            {
                context.Set("mongo.collection", context.Request.Uri.Segments[mongoIndex + 1].TrimEnd("/".ToCharArray()));
                Debug.WriteLine("Collection: " + context.Get<string>("mongo.collection"));

                var mongoRepository = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IMongoRepository)) as IMongoRepository;

                Debug.WriteLine(mongoRepository.Database.GetStats());
            }

            if (context.Request.User != null && context.Request.User.IsInRole("admin"))
            {

            }

            await _next(environment);
        }
    }
}