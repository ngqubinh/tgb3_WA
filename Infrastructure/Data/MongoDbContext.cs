using Domain.Models.Auth;
using Domain.Models.Management;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Data
{
    public class MongoDBContext 
    {
        private readonly IMongoDatabase _mongo;
        // public MongoDBContext(IMongoDatabase mongo)
        // {
        //     this._mongo = mongo;
        // }

        public MongoDBContext(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("MongoDB");
            var client = new MongoClient(connectionString);
            _mongo = client.GetDatabase("ShelkovyPutChat");
        }

        public IMongoCollection<OMessage> Messages => _mongo.GetCollection<OMessage>("Messages");
        public IMongoCollection<User> Users => _mongo.GetCollection<User>("Users");
    }
}