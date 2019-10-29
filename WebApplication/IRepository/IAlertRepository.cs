using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;
using System.Linq;

namespace WebApplication.IRepository
{
    public interface IAlertRepository
    {
        List<Alert> Get();
        Alert Get(string id);
        IQueryable<Alert> GetAll();
        IQueryable<Alert> GetByName(string searchTerm);

        bool Add(Alert item);
        Task<IEnumerable<Alert>> GetAllAlerts();
        Task<Alert> GetAlert(string id);
        Task<bool> AddAlert(Alert item);
        Task<bool> UpdateAlert(string id, Alert item);
        Task<bool> RemoveAlert(string id);
        // Task<IEnumerable<Condition>> GetConditions(string alertId);
        // Task<Condition> getCondition(string alertId, string conditionId);
        // Task<bool> AddCondition(string id);
        Task<bool> editCondition(string alertId, string conditionId, Condition condition);
        // Task <bool> deleteCondition(string alertId, string conditionId);
        Condition getConditionObject(string alertId, string conditionId);
        Task<bool> updateAlertStatus(string alertId, Boolean Status);
        Task<bool> updateLastChecked(string alertId, long lastChecked);

    }
}