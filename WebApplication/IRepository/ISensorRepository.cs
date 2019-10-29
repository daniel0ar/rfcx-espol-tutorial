using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;
using MongoDB.Bson;

namespace WebApplication.IRepository
{
    public interface ISensorRepository
    {
        Task<IEnumerable<Sensor>> Get();
        Task<Sensor> Get(string id);
        Task<Sensor> Get(int id);
        Task<Sensor> Get(int StationId, int SensorId);
        Sensor getSensor(int id);
        Sensor getSensorByStation(int StationId, int SensorId);
        Task<IEnumerable<Sensor>> GetByStation(int StationId);        
        IEnumerable<Sensor> GetByStationNotAsync(int StationId);
        Task Add(Sensor item);
        Task<bool> Update(string id, Sensor item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();        
        List<BsonDocument> sensorsTypeAndLocation();
    }
}