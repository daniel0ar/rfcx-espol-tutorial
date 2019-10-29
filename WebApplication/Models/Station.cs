using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

using System.Linq;

using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Station : IComparable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StationId { get; set; }
        public int Id { get; set; }
        public string APIKey { get; set; }
        public string Name { get; set; }
        public int GameStation { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AndroidVersion { get; set; }
        public string ServicesVersion { get; set; }

        public int CompareTo(object obj)
        {
        Station station = obj as Station;
        if(station == null) return 1;
        if (station.Id < Id)
        {
            return 1;
        }
        if (station.Id > Id)
        {
            return -1;
        }

        return 0;
        }
    }
    
}