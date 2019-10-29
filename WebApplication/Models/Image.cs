using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;




namespace WebApplication.Models
{
    [BsonIgnoreExtraElements]
    public class Image
    {
        public static IMongoClient client = new MongoClient(Constants.MONGO_CONNECTION);
        public static IMongoDatabase _database = client.GetDatabase(Constants.DEFAULT_DATABASE_NAME);
        public static IMongoCollection<Image> collection = _database.GetCollection<Image>("Camera_Image");
        
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId id {get; set;}
        public string ImageId { get; set; }
        public int Id {get; set;}
        public int StationId{ get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CaptureDate{ get; set; }

        public DateTime ArriveDate{ get; set; }
        
        public string Path{ get; set; }
        public string State{ get; set; }

        public string[] Tag { get; set; }
        //public JsonResult Predictions {get; set;}
        //public List<String> Family{ get; set; }
        
        //public List<String> LabelList { get; set; }
        
        public Image(){}
        public Image(int IdEstacion, string FechaCaptura, string Extension)
        {
            id = ObjectId.GenerateNewId();
            this.StationId = IdEstacion;
            this.CaptureDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(FechaCaptura)).DateTime;
            this.ArriveDate = DateTime.UtcNow;
            Path = id + Extension;
            State = "PENDIENTE";
            
        }
        
    }
    
}