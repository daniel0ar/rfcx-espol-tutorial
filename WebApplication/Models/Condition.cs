using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Condition
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string StationId { get; set; }
        public string SensorId { get; set; }
        public double Threshold { get; set; }
        public string Comparison { get; set; }

        public Condition()
        {
            _id = ObjectId.GenerateNewId();
        }


    }

}