using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace WebApplication.Repository
{
    public class InfoSensoresRepository : IInfoSensoresRepository
    {
        private readonly ObjectContext _context =null; 

        public InfoSensoresRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<InfoSensores>> Get()
        {
            try
            {
                return await _context.InfoSensoress.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<InfoSensores> Get(string id)
    {
        var filter = Builders<InfoSensores>.Filter.Eq("InfoSensoresId", id);

        try
        {
            return await _context.InfoSensoress.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(InfoSensores item)
    {
        try
        {
            item.Id=_context.InfoSensoress.Find(_ => true).ToList().Count+1;
            await _context.InfoSensoress.InsertOneAsync(item);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Remove(string id)
    {
        try
        {
            DeleteResult actionResult = await _context.InfoSensoress.DeleteOneAsync(
                    Builders<InfoSensores>.Filter.Eq("InfoSensoresId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    
    public async Task<bool> Update(string id, InfoSensores item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.InfoSensoress
                                .ReplaceOneAsync(n => n.InfoSensoresId.Equals(id)
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
                = await _context.InfoSensoress.DeleteManyAsync(new BsonDocument());

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    }
}