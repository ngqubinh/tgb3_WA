using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Auth;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Management
{
    public class Messages
    {
        [BsonId]
        // [Key]
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}