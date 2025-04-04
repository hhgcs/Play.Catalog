using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Play.Catalog.Service.Repositories
{

    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;
        public MongoDBContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
            var _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            if (_database == null) throw new NullReferenceException("MongoDB Database is not initialized.");

            var collectionList = _database.ListCollectionNames().ToList();
            if (!collectionList.Contains(collectionName))
            {
                _database.CreateCollection(collectionName);
            }
            return _database.GetCollection<T>(collectionName);
        }
    }
}