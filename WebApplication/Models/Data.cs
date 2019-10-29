using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Data
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DataId { get; set; }
        public int StationId { get; set; }
        public int SensorId { get; set; }
        public int Id { get; set; }
        public long Timestamp { get; set; }
        public long SavedAt { get; set; }
        public string Type { get; set; }
        public double Value { get; set; }
        public string Units { get; set; }
        public string Location { get; set; }


        public Data()
        {
            SavedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

    }
}