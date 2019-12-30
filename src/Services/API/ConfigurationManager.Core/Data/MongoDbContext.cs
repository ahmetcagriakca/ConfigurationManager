using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManager.Core.Data
{
    public class MongoDbContext : IDisposable
    {
        public IMongoClient Client;
        public IMongoDatabase Database;


        public MongoDbContext(string connectionString,string database)
        {
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(database);
        }

        public bool CheckConnection()
        {
            bool isMongoLive = Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
            return isMongoLive;
        }
        public void Dispose()
        {
        }
    }
}
