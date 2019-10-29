using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Animal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AnimalId { get; set; }
        public string Name { get; set; }
        public string ScientificName { get; set; }
    }
}