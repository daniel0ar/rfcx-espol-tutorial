using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using System.IO;

namespace WebApplication.Repository
{
    public class AudioRepository : IAudioRepository
    {
        private readonly ObjectContext _context =null; 

        public AudioRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Audio>> Get()
        {
            try
            {
                return await _context.Audios.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Audio> Get(string id)
        {
            var filter = Builders<Audio>.Filter.Eq("AudioId", id);

            try
            {
                return await _context.Audios.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Audio> Get(int id)
        {
            var filter = Builders<Audio>.Filter.Eq("Id", id);

            try
            {
                return await _context.Audios.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Audio>> GetByStation(int StationId)
        {
            try
            {
                var filter =Builders<Audio>.Filter.Eq("StationId", StationId);
                return await _context.Audios.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Audio> Get(int StationId, int AudioId)
        {
            var filter = Builders<Audio>.Filter.Eq("Id", AudioId) & Builders<Audio>.Filter.Eq("StationId", StationId);

            try
            {
                return await _context.Audios.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Add(Audio item)
        {
            try
            {
                var list=_context.Audios.Find(_ => true).ToList();
                if(list.Count>0){
                    item.Id=list[list.Count-1].Id+1;
                }else{
                    item.Id=1;
                }
    
                await _context.Audios.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Remove(int StationId, int AudioId)
        {
            try
            {
                DeleteResult actionResult = await _context.Audios.DeleteOneAsync(
                        Builders<Audio>.Filter.Eq("Id", AudioId));

                string[] audios;

                string audioNameToDelete = AudioId + ".m4a";
                string audioNameOggToDelete = AudioId + ".ogg";

                string audiosDeletePath = Core.StationAudiosFolderPath(StationId.ToString());
                string audiosOggDeletePaht = Core.StationOggFolderPath(StationId.ToString());

                if (Directory.Exists(audiosDeletePath))
                {
                    audios = Directory.GetFiles(audiosDeletePath);

                    foreach (string audio in audios)
                    {
                        string audioName = Path.GetFileName(audio);
                        if (audioNameToDelete == audioName)
                            File.Delete(audio);
                    }
                }

                if (Directory.Exists(audiosOggDeletePaht))
                {
                    audios = Directory.GetFiles(audiosOggDeletePaht);

                    foreach (string audio in audios)
                    {
                        string audioName = Path.GetFileName(audio);
                        if (audioNameOggToDelete == audioName)
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

        public async Task<bool> Update(int StationId, int AudioId, Audio item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Audios
                                    .ReplaceOneAsync(n => n.AudioId.Equals(AudioId)
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
                    = await _context.Audios.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Audio> GetByStationAndDate(int StationId, DateTime Start, DateTime End)
        {
            try
            {
                return _context.Audios.AsQueryable().Where(a => a.RecordingDate >= Start).Where(a => a.RecordingDate <= End).Where(a => a.StationId == StationId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Audio> GetLastAudio()
        {
            try
            {
                var filter = Builders<Audio>.Filter.Exists("Id");
                var sort = Builders<Audio>.Sort.Descending("Id");
                return await _context.Audios.Find(filter).Sort(sort).FirstAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddTag(int AudioId, string Tag)
        {
            try
            {
                var filter = Builders<Audio>.Filter.Eq("Id", AudioId);
                var update = Builders<Audio>.Update.Push("LabelList", Tag);

                UpdateResult updateResult = await _context.Audios.UpdateOneAsync(filter, update);

                return updateResult.IsAcknowledged;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}