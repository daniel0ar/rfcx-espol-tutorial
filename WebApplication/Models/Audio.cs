using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;


namespace WebApplication.Models
{

    public class Audio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AudioId { get; set; }
        public int Id { get; set; }
        public int StationId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime RecordingDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ArriveDate { get; set; }
        public string Duration { get; set; }
        public string Format { get; set; }
        public int BitRate { get; set; }
        public List<String> LabelList { get; set; }
    }
    
}