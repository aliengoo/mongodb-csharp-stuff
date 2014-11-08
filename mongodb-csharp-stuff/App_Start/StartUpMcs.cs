namespace Mcs
{
    using System.Configuration;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;
    using System.Web.Http.ModelBinding.Binders;

    using Mcs.Converters;
    using Mcs.Helpers;
    using Mcs.ModelBinders;

    using MongoDB.Bson;

    using Owin;

    public static class StartUpMcs
    {
        /// <summary>
        /// Register MongoDB type features with OWIN
        /// </summary>
        /// <remarks>
        /// <seealso cref="BsonDocumentJsonConverter"/> is added to JSON formatters
        /// <seealso cref="RemoteMongoQueryConverter"/> is added to JSON formatters
        /// <seealso cref="ObjectIdModelBinder"/> <seealso cref="SimpleModelBinderProvider"/> is added to services.
        /// </remarks>
        /// <param name="appBuilder"></param>
        /// <param name="configuration"></param>
        /// <param name="databaseConnectionStringName">When present, a new MongoDB connection gets created and set on property "mongodb.{databaseConnectionStringName}"</param>
        public static void UseMcs(this IAppBuilder appBuilder, HttpConfiguration configuration, string databaseConnectionStringName = null)
        {
            // init formatting of JSON request/responses
            var formatters = configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.Converters.Add(new BsonDocumentJsonConverter());
            jsonFormatter.SerializerSettings.Converters.Add(new RemoteMongoQueryConverter());

            var objectIdModelBinderProvider = new SimpleModelBinderProvider(typeof(ObjectId), new ObjectIdModelBinder());
            configuration.Services.Insert(typeof(ModelBinderProvider), 0, objectIdModelBinderProvider);

            if (!string.IsNullOrWhiteSpace(databaseConnectionStringName))
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings[databaseConnectionStringName];

                if (connectionStringSettings == null)
                {
                    throw new ConfigurationErrorsException(string.Format(
                        "Mcs Error: Could not locate connection string with name '{0}'.", databaseConnectionStringName));
                }

                var db = MongoDbHelper.GetDatabase(databaseConnectionStringName);

                appBuilder.Properties.Add("mongodb." + databaseConnectionStringName, db);
                
            }
        }
    }
}