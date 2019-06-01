using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Services
{
    public class MongoDbCdnContext
    {
        public IGridFSBucket GridFsBucket { get; }
        public MongoClient Client { get; }
        protected MongoDbCdnContext(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("MongoDb");
            var connection = new MongoUrl(connectionString);
            var settings = MongoClientSettings.FromUrl(connection);

            Client = new MongoClient(settings);
            var database = Client.GetDatabase(connection.DatabaseName);
            GridFsBucket = new GridFSBucket(database);
        }
    }
}
