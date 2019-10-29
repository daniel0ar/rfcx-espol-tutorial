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
    public class AlertRepository : IAlertRepository
    {
        private readonly ObjectContext _context = null;

        public AlertRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        }


        public async Task<IEnumerable<Alert>> GetAllAlerts()
        {
            try
            {
                return await _context.Alerts.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Alert> GetAlert(string id)
        {
            var filter = Builders<Alert>.Filter.Eq("_id", ObjectId.Parse(id));

            try
            {
                return await _context.Alerts.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddAlert(Alert item)
        {
            try
            {
                var list = _context.Alerts.Find(_ => true).ToList();
                // if (list.Count > 0)
                // {
                //     item.Id = list[list.Count - 1].Id + 1;
                // }
                // else
                // {
                //     item.Id = 1;
                // }

                await _context.Alerts.InsertOneAsync(item);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAlert(string id)
        {
            try
            {
                DeleteResult actionResult = await _context.Alerts.DeleteOneAsync(
                        Builders<Alert>.Filter.Eq("_id", ObjectId.Parse(id)));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> UpdateAlert(string id, Alert item)
        {
            try
            {
                ReplaceOneResult actionResult
                    = await _context.Alerts
                                    .ReplaceOneAsync(n => n.AlertId.Equals(id)
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


        public Alert getAlertObject(string id)
        {
            var filter = Builders<Alert>.Filter.Eq("_id", ObjectId.Parse(id));
            try
            {
                Alert alert = _context.Alerts.Find(filter).FirstOrDefaultAsync().Result;
                return alert;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Condition getConditionObject(string alertId, string conditionId)
        {
            Alert alert = getAlertObject(alertId);
            Condition condition = null;
            foreach (Condition c in alert.Conditions)
            {
                if (c._id == ObjectId.Parse(conditionId))
                {
                    condition = c;
                }
            }
            return condition;
        }

        public List<Alert> Get()
        {
            try
            {
                return _context.Alerts.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Alert Get(string id)
        {
            var filter = Builders<Alert>.Filter.Eq("_id", ObjectId.Parse(id));

            try
            {
                Console.WriteLine(_context.Alerts.Find(filter).FirstOrDefault().AlertId);
                return _context.Alerts.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Alert> GetAll()
        {
            try
            {
                return _context.Alerts.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Alert> GetByName(String searchTerm)
        {
           try
            {
                return _context.Alerts.AsQueryable().Where(a => a.Name.Contains(searchTerm));
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        public bool Add(Alert item)
        {
            try
            {
                var list = _context.Alerts.Find(_ => true).ToList();
                _context.Alerts.InsertOne(item);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Write(ex.Source);
                Console.Write(ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> editCondition(string alertId, string conditionId, Condition condition)
        {
            var filter = Builders<Alert>.Filter;
            var AlertIdAndConditionIdFilter = filter.And(
                filter.Eq(x => x.AlertId, alertId),
                filter.ElemMatch(x => x.Conditions, c => c._id == ObjectId.Parse(conditionId)));
            var update = Builders<Alert>.Update;
            var conditionSetter = update.Set("Conditions.$", condition);
            UpdateResult actionResult = await _context.Alerts.UpdateOneAsync(AlertIdAndConditionIdFilter, conditionSetter);
            return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }

        public async Task<bool> updateAlertStatus(string alertId, Boolean status)
        {
            var filter = Builders<Alert>.Filter.Eq("_id", ObjectId.Parse(alertId));
            var update = Builders<Alert>.Update.Set("Status", status);

            try
            {
                UpdateResult actionResult = await _context.Alerts.UpdateOneAsync(filter, update);
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> updateLastChecked(string alertId, long lastChecked)
        {
            var filter = Builders<Alert>.Filter.Eq("_id", ObjectId.Parse(alertId));
            var update = Builders<Alert>.Update.Set("LastChecked", lastChecked);

            try
            {
                UpdateResult actionResult = await _context.Alerts.UpdateOneAsync(filter, update);
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