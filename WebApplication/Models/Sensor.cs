using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebApplication.Models
{

    public class Sensor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SensorId { get; set; }
        public int Id { get; set; }
        public int StationId { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
    }
    
}