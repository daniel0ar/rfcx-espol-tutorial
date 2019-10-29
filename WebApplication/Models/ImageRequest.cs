using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
namespace WebApplication.Models
{
    public class ImageRequest
    {
        
        public string CaptureDate {get;set;}
        public int StationId{get;set;}
        public IFormFile ImageFile{get;set;}
       
        public string APIKey{get;set;}

        public List<string> Family{get;set;}
        public string Base64Image{get;set;}
    }
    
}