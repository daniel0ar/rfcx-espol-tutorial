using WebApplication.IRepository;
using WebApplication.DbModels;
using WebApplication.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;




namespace WebApplication.Repository
{
    public class ImageRepository : IImageRepository
    {
        private readonly ObjectContext _context = null;

        public ImageRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        }

        public async Task<Image> Find(string _id)
        {
            var filter = "{'_id':" +  "ObjectId('"+_id + "')}";
            var imgDB = await _context.Images.Find(filter).Limit(1).FirstOrDefaultAsync();
            return imgDB;
        }


       public async Task<IEnumerable<Image>> Get()
        {
            try
            {
                return await _context.Images.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Image> Get(string id)
        {
            var filter = Builders<Image>.Filter.Eq("ImageId", id);

            try
            {
                return await _context.Images.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Image> Get(int id)
        {
            var filter = Builders<Image>.Filter.Eq("Id", id);

            try
            {
                return await _context.Images.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Image>> GetByStation(int StationId)
        {
            try
            {
                var filter =Builders<Image>.Filter.Eq("StationId", StationId);
                return await _context.Images.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Image> Get(int StationId, int ImageId)
        {
            var filter = Builders<Image>.Filter.Eq("Id", ImageId) & Builders<Image>.Filter.Eq("StationId", StationId);

            try
            {
                return await _context.Images.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Add(Image item)
        {
            try
            {
                var list=_context.Images.Find(_ => true).ToList();
                if(list.Count>0){
                    item.Id=list[list.Count-1].Id+1;
                }else{
                    item.Id=1;
                }
    
                await _context.Images.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Remove(int StationId, int ImageId)
        {
            try
            {
                DeleteResult actionResult = await _context.Images.DeleteOneAsync(
                        Builders<Image>.Filter.Eq("Id", ImageId));

                string[] imagenes;

                string imageNameToDelete = ImageId + ".jpg";
                string imageNameOggToDelete = ImageId + ".ogg";

                string imagesDeletePath = Core.StationImagesFolderPath(StationId.ToString());
                string imagesOggDeletePaht = Core.StationOggFolderPathImage(StationId.ToString());

                if (Directory.Exists(imagesDeletePath))
                {
                    imagenes = Directory.GetFiles(imagesDeletePath);

                    foreach (string audio in imagenes)
                    {
                        string audioName = Path.GetFileName(audio);
                        if (imageNameToDelete == audioName)
                            File.Delete(audio);
                    }
                }

                if (Directory.Exists(imagesOggDeletePaht))
                {
                    imagenes = Directory.GetFiles(imagesOggDeletePaht);

                    foreach (string audio in imagenes)
                    {
                        string audioName = Path.GetFileName(audio);
                        if (imageNameOggToDelete == audioName)
                            File.Delete(audio);
                    }
                }

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(int StationId, int ImageId, Image item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Images
                                    .ReplaceOneAsync(n => n.ImageId.Equals(ImageId)
                                            , item
                                            , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAll()
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.Images.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Image> GetByStationAndDate(int StationId, DateTime Start, DateTime End)
        {
            try
            {
                return _context.Images.AsQueryable().Where(a => a.CaptureDate >= Start).Where(a => a.CaptureDate <= End).Where(a => a.StationId == StationId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Image> GetLastImage()
        {
            try
            {
                var filter = Builders<Image>.Filter.Exists("Id");
                var sort = Builders<Image>.Sort.Descending("Id");
                return await _context.Images.Find(filter).Sort(sort).FirstAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddTag(int ImageId, string Tag)
        {
            try
            {
                var filter = Builders<Image>.Filter.Eq("Id", ImageId);
                var update = Builders<Image>.Update.Push("Tag", Tag);

                UpdateResult updateResult = await _context.Images.UpdateOneAsync(filter, update);

                return updateResult.IsAcknowledged;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

         
        private bool IsApiKeyCorrect(string ApiKey)
        {
            var filter = Builders<Station>.Filter.Eq("APIKey", ApiKey);
            return _context.Stations.Find(filter).Any();
        }
        public async Task<ActionResult> PostPicture(ImageRequest req)
        {
            if(IsApiKeyCorrect(req.APIKey)){
                string extension = System.IO.Path.GetExtension(req.ImageFile.FileName);
                Image img = new Image(req.StationId, req.CaptureDate, extension);
                var imgPath = Constants.RUTA_ARCHIVOS_ANALISIS_IMAGENES + img.StationId + "/" + img.Path;
                new FileInfo(imgPath).Directory.Create();
                using(FileStream stream = new FileStream(imgPath, FileMode.Create)){
                    await req.ImageFile.CopyToAsync(stream);
                }
                _context.Images.InsertOne(img);
                return new ContentResult()
                {
                    Content = "{\"_id\": \"" + img.id + "\"}",
                    ContentType="application/json"
                };
            }else{
                return new StatusCodeResult(500);
            }
        }

        public async Task<List<Image>> ListImages(DateTime starttime, DateTime endtime, int page, int rows)
        {
            var filterBuilder = Builders<Image>.Filter;
            var start = starttime;
            var end = endtime;
            var filter = filterBuilder.Gte(x => x.CaptureDate, new BsonDateTime(start)) & filterBuilder.Lte(x => x.CaptureDate, new BsonDateTime(end));
            var arr = new List<Image>();
            await _context.Images.Find(filter).ForEachAsync(
                img =>
                {
                    arr.Add(img);
                });
            var lower = rows * (page - 1);
            if (lower >= arr.Count)
                return new List<Image>();
            var upper = lower + rows;
            if(upper > arr.Count)
                upper = arr.Count;  
            return arr.GetRange(lower, upper-lower);


        }
    }
    /*
    */
}