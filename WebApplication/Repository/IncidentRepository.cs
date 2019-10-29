using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication.DbModels;
using WebApplication.IRepository;
using WebApplication.Models;
using System.Linq;


namespace WebApplication.Repository
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly ObjectContext _context = null;

        public IncidentRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        }

        public async Task AddIncident(Incident item)
        {
            try
            {
                await _context.Incidents.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Incident>> GetAllIncidents()
        {
            try
            {
                return await _context.Incidents.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Incident> GetIncident(string id)
        {
            var filter = Builders<Incident>.Filter.Eq("_id", ObjectId.Parse(id));

            try
            {
                return await _context.Incidents.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveIncident(string id)
        {
            var filter = Builders<Incident>.Filter.Eq("_id", ObjectId.Parse(id));
            try
            {
                DeleteResult actionResult = await _context.Incidents.DeleteOneAsync(filter);

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateIncident(string id, Incident item)
        {
            var filter = Builders<Incident>.Filter.Eq("_id", ObjectId.Parse(id));
            try
            {
                ReplaceOneResult actionResult
                    = await _context.Incidents.ReplaceOneAsync(filter, item, new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<bool> UpdateIncidentStatus(string id, Boolean status)
        {
            var filter = Builders<Incident>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<Incident>.Update.Set("Status", status);

            try
            {
                UpdateResult actionResult = await _context.Incidents.UpdateOneAsync(filter, update);
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Incident> Get()
        {
            try
            {
                return _context.Incidents.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IQueryable<Incident> GetAll()
        {
            try
            {
                return _context.Incidents.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<Incident> GetByDate(DateTime start, DateTime end)
        {
            try
            {
                return _context.Incidents.AsQueryable().Where(i => i.IncidentTime >= start).Where(i => i.IncidentTime <= end);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}