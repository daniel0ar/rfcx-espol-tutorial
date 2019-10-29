using MongoDB.Driver;
using System.Collections.Generic;
using WebApplication.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;


namespace WebApplication.IRepository
{
    public interface IImageRepository
    {
       

        Task<IEnumerable<Image>> Get();
       
        Task<Image> Get(string id);
        

       Task<Image> Get(int id);
        

        Task<IEnumerable<Image>> GetByStation(int StationId);
                
        Task<Image> Get(int StationId, int ImageId);
        
       Task Add(Image item);
        Task<bool> Remove(int StationId, int ImageId);
        
        Task<bool> Update(int StationId, int ImageId, Image item);
        
        Task<bool> RemoveAll();
        
        IQueryable<Image> GetByStationAndDate(int StationId, DateTime Start, DateTime End);
        
        Task<Image> GetLastImage();
        
       Task<bool> AddTag(int ImageId, string Tag);

       Task<Image> Find(string _id);
       
        Task<ActionResult> PostPicture(ImageRequest req);
        Task<List<Image>> ListImages(DateTime starttime, DateTime endtime, int page, int rows);

        
    }   

              
}