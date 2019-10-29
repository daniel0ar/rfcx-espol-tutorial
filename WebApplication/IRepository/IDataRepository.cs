using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;
using MongoDB.Bson;

namespace WebApplication.IRepository
{
    public interface IDataRepository
    {
        Task<IEnumerable<Data>> Get();
        Task<Data> Get(string id);
        Task<Data> Get(int id);
        Task<Data> Get(int StationId, int SensorId, int DataId);
        Task<IEnumerable<Data>> GetLasts();
        Task<IEnumerable<Data>> GetByStation(int StationId);
        Task<Data> GetLastByStation(int StationId);

        Task<IEnumerable<Data>> GetByStationSensor(int StationId, int SensorId);
        Task<Data> GetLastByStationSensor(int StationId, int SensorId);
        Task<IEnumerable<Data>> GetByStationSensorTimestamp(int StationId, int SensorId, long StartTimestamp, long EndTimestamp);
        Task<IEnumerable<Data>> GetByStationSensorTimestampFilter(int StationId, int SensorId, 
            long StartTimestamp, long EndTimeStamp, string Filter, int FilterValue);
        Task Add(Data item);
        Task<bool> Update(string id, Data item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();

        Task<IEnumerable<Data>> DataByStationTimestamp(
            int StationId, 
            long StartTimestamp, 
            long EndTimestamp
        );
        Task<IEnumerable<BsonDocument>> AvgPerDate(
            int StationId,
            long StartTimestamp,
            long EndTimestamp
        );

        Task<IEnumerable<BsonDocument>> AvgPerHour(
            int StationId,
            long StartTimestamp,
            long EndTimestamp
        );    

        Task<IEnumerable<BsonDocument>> AvgPerMonth(
            int StationId,
            long StartTimestamp,
            long EndTimestamp
        ); 

        List<BsonDocument> sensorsTypeAndLocation();

        Task<IEnumerable<BsonDocument>> AvgPerDateStation(
            string SensorType,
            string SensorLocation,
            long StartTimestamp,
            long EndTimestamp
        ); 
    }
}