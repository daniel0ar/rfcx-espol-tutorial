using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;
using System.Linq;

namespace WebApplication.IRepository
{
    public interface IIncidentRepository
    {
        List<Incident> Get();
        Task<IEnumerable<Incident>> GetAllIncidents();
        Task<Incident> GetIncident(string id);
        IQueryable<Incident> GetAll();
        IQueryable<Incident> GetByDate(DateTime Start, DateTime End);

        Task AddIncident(Incident item);
        Task<bool> UpdateIncident(string id, Incident item);
        Task<bool> RemoveIncident(string id);
        Task<bool> UpdateIncidentStatus(string id, Boolean status);

    }
}