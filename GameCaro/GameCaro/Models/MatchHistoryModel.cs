using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro.Models
{
    public class MatchHistoryModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("playerX")]
        public string PlayerX { get; set; }

        [BsonElement("playerO")]
        public string PlayerO { get; set; }

        [BsonElement("winner")]
        [BsonIgnoreIfNull]
        public string Winner { get; set; }

        [BsonElement("reason")]
        public string Reason { get; set; }

        [BsonElement("moveCount")]
        public int MoveCount { get; set; }

        [BsonElement("startedAtUtc")]
        public DateTime StartedAtUtc { get; set; }

        [BsonElement("finishedAtUtc")]
        public DateTime FinishedAtUtc { get; set; }

        [BsonElement("durationSeconds")]
        public int DurationSeconds { get; set; }
    }
}
