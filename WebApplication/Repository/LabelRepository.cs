using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver;



namespace WebApplication.Repository
{
    public class LabelRepository : ILabelRepository
    {
        private readonly ObjectContext _context =null; 

        public LabelRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public async Task<IEnumerable<Label>> Get()
        {
            try
            {
                return await _context.Labels.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Label> Get(string id)
    {
        var filter = Builders<Label>.Filter.Eq("LabelId", id);

        try
        {
            return await _context.Labels.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(Label item)
    {
        try
        {
            var list=_context.Labels.Find(_ => true).ToList();
            if(list.Count>0){
                item.Id=list[list.Count-1].Id+1;
            }else{
                item.Id=1;
            }

            await _context.Labels.InsertOneAsync(item);
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
            DeleteResult actionResult = await _context.Labels.DeleteOneAsync(
                    Builders<Label>.Filter.Eq("LabelId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    

    public async Task<bool> Update(string id, Label item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Labels
                                .ReplaceOneAsync(n => n.LabelId.Equals(id)
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
                = await _context.Labels.DeleteManyAsync(new BsonDocument());

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