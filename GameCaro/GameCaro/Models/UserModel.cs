using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro.Models
{
    public class UserModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("score")]
        public int Score { get; set; }

        [BsonElement("totalGames")]
        public int TotalGames { get; set; }

        [BsonElement("createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }

        [BsonElement("lastLoginAtUtc")]
        [BsonIgnoreIfNull]
        public DateTime? LastLoginAtUtc { get; set; }
    }
}
