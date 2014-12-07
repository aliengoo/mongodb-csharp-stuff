namespace Mcs.Helpers
{
    using System.Configuration;

    using MongoDB.Driver;

    public static class MongoDbHelper
    {

        public static MongoDatabase GetDatabaseByConnectionString(string connectionString)
        {
            var url = new MongoUrl(connectionString);

            if (string.IsNullOrWhiteSpace(url.DatabaseName))
            {
                throw new ConfigurationErrorsException(string.Format("The connection {0} does not appear to have a database name", connectionString));
            }

            return new MongoClient(url).GetServer().GetDatabase(url.DatabaseName);
        }

        public static MongoDatabase GetDatabase(string connectionStringName)
        {
            var connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSetting == null)
            {
                throw new ConfigurationErrorsException(string.Format("Connection {0} was not defined", connectionStringName));
            }

            return GetDatabaseByConnectionString(connectionStringSetting.ConnectionString);
        }
    }
}