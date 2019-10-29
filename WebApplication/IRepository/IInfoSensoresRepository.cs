using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IInfoSensoresRepository
    {
        Task<IEnumerable<InfoSensores>> Get();
        Task<InfoSensores> Get(string id);
        Task Add(InfoSensores item);
        Task<bool> Update(string id, InfoSensores item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}