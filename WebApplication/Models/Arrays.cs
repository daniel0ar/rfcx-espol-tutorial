using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Arrays
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<Data> Data { get; set; }
        public int IdPhoto { get; set; }
        public string APIKey { get; set; }
        public string AndroidVersion { get; set; }
        public string ServicesVersion { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LastNotification { get; set; }
        public string Status { get; set; }
        public int Gamestation { get; set; }
        public string Family { get; set; }
        public string Description { get; set; }
        public int SpecieId { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }
        public string Option { get; set; }
        public int Answer { get; set; }
        public string Feedback { get; set; }
        public string Category { get; set; }
        public string Stations { get; set; }
        }
}