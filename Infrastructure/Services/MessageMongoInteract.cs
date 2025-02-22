using Domain.Models.Management;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Infrastructure.Services
{
    public class MessageMongoInteract
    {
        private readonly IMongoCollection<OMessage> _messageMongo;
        public MessageMongoInteract(MongoDBContext context)
        {
            this._messageMongo = context.Messages;
        }

        public async Task AddMessageAsync(OMessage messages)
        {
            await _messageMongo.InsertOneAsync(messages);
        }
    }
}