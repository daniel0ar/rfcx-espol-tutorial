using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Label
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)] 
            public string LabelId { get; set; }
            public string AudioId {get; set; }
            public int Id { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string Description { get; set; }
        }
}