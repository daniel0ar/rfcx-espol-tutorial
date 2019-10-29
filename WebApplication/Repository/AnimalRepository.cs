using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;


namespace WebApplication.Repository
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ObjectContext _context = null;
        public AnimalRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 
        public async Task<bool> Add(Animal item)
        {
            try
            {
                var list = _context.Animals.Find(_ => true).ToList();
                await _context.Animals.InsertOneAsync(item);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Animal> GetAll(){
            try
            {
                return _context.Animals.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Animal>> Get()
        {
            try
            {
                return await _context.Animals.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Animal> Get(string id)
        {
            var filter = Builders<Animal>.Filter.Eq("AnimalId", id);

            try
            {
                return await _context.Animals.Find(filter).FirstOrDefaultAsync();
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
                DeleteResult actionResult = await _context.Animals.DeleteOneAsync(
                        Builders<Animal>.Filter.Eq("_id", ObjectId.Parse(id)));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(string id, Animal item)
        {
            try
            {
                ReplaceOneResult actionResult
                    = await _context.Animals
                                    .ReplaceOneAsync(n => n.AnimalId.Equals(id)
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
    }
}