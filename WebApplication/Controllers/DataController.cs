using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Services;
using MongoDB.Bson;


namespace WebApplication.Controllers
{
        [Route("")]
        public class DataController
        {
            
            private readonly IDataRepository _DataRepository;
            private readonly ISensorRepository _SensorRepository;

            public DataController(IDataRepository DataRepository,  ISensorRepository SensorRepository)
            {
                _DataRepository=DataRepository;
                _SensorRepository=SensorRepository;
            }

            [HttpGet]
            [Route("api/[controller]")]
            public Task<string> Get()
            {
                return this.GetData();
            }

            private async Task<string> GetData()
            {
                var Datas= await _DataRepository.Get();
                return JsonConvert.SerializeObject(Datas);
            }

            /*
            [HttpGet]
            [Route("api/[controller]/{id}")]
            public Task<string> Get(string id)
            {
                return this.GetDataById(id);
            }

            public async Task<string> GetDataById(string id)
            {
                var Data= await _DataRepository.Get(id) ?? new Data();
                return JsonConvert.SerializeObject(Data);
            }

            */
            [HttpGet]
            [Route("api/[controller]/{id:int}")]
            public Task<string> Get(int id)
            {
                return this.GetDataByIdInt(id);
            }
            
            [HttpGet]
            [Route("api/[controller]/lastData")]
            public Task<string> GetLasts()
            {
                return this.GetLast();
            }

            private async Task<string> GetLast()
            {
                var Datass= await _DataRepository.GetLasts();
                return JsonConvert.SerializeObject(Datass);
            }

            private async Task<string> GetDataByIdInt(int id)
            {
                var Data= await _DataRepository.Get(id) ?? new Data();
                return JsonConvert.SerializeObject(Data);
            }

            [HttpGet]
            [Route("api/Station/{StationId:int}/[controller]")]
            public Task<string> GetDatasByStation([FromRoute]int StationId)
            {
                return this.GetDataByStation(StationId);
            }

            private async Task<string> GetDataByStation(int StationId)
            {
                var Datass= await _DataRepository.GetByStation(StationId);
                return JsonConvert.SerializeObject(Datass);
            }

            [HttpGet]
            [Route("api/Station/{StationId:int}/Sensor/{SensorId:int}/[controller]")]
            public Task<string> GetDatasByStationSensor([FromRoute]int StationId,[FromRoute] int SensorId)
            {
                return this.GetDataByStationSensor(StationId, SensorId);
            }

            private async Task<string> GetDataByStationSensor(int StationId, int SensorId)
            {
                var Datas= await _DataRepository.GetByStationSensor(StationId, SensorId);
                return JsonConvert.SerializeObject(Datas);
            }

            [HttpGet]
            [Route("api/Station/{StationId:int}/Sensor/{SensorId:int}/[controller]/lastData")]
            public Task<string> GetLastDataByStationSensor([FromRoute]int StationId,[FromRoute] int SensorId)
            {
                return this.GetLastsDataByStationSensor(StationId, SensorId);
            }

            private async Task<string> GetLastsDataByStationSensor(int StationId, int SensorId)
            {
                var Datas= await _DataRepository.GetLastByStationSensor(StationId, SensorId);
                return JsonConvert.SerializeObject(Datas);
            }

            [HttpGet]
            [Route("api/Station/{StationId:int}/[controller]/lastData")]
            public Task<string> GetLastDataByStation([FromRoute]int StationId)
            {
                return this.GetLastsDataByStation(StationId);
            }

            private async Task<string> GetLastsDataByStation(int StationId)
            {
                var Datas= await _DataRepository.GetLastByStation(StationId);
                return JsonConvert.SerializeObject(Datas);
            }

            [HttpGet]
            [Route("api/Station/{StationId:int}/Sensor/{SensorId:int}/DataTimestamp")]
            public Task<string> GetDatasByStationSensorTimestamp([FromRoute]int StationId,[FromRoute] int SensorId, 
            [FromQuery] long StartTimestamp, [FromQuery] long EndTimestamp)
            {
                return this.GetDataByStationSensorTimeStamp(StationId, SensorId, StartTimestamp, EndTimestamp);
            }


            [HttpGet]
            [Route("api/Station/{StationId:int}/Timestamp")]
            public Task<string> GetDataByStationTimestamp(
                [FromRoute] int StationId,
                [FromQuery] long StartTimestamp,
                [FromQuery] long EndTimestamp
            )
            {
                return this._GetDataByStationTimestamp(
                    StationId,
                    StartTimestamp, 
                    EndTimestamp
                );
            }
            
            private async Task<string> _GetDataByStationTimestamp(
                int StationId,
                long StartTimestamp, 
                long EndTimestamp
            )
            {
                var data = await _DataRepository.DataByStationTimestamp(
                    StationId, 
                    StartTimestamp, 
                    EndTimestamp
                );
                return JsonConvert.SerializeObject(data);
            }

            private async Task<string> GetDataByStationSensorTimeStamp(int StationId, int SensorId, long StartTimestamp, long EndTimeStamp)
            {
                var Datas= await _DataRepository.GetByStationSensorTimestamp(StationId, SensorId, StartTimestamp, EndTimeStamp);
                return JsonConvert.SerializeObject(Datas);
            }

            [HttpGet]
            [Route("api/Station/{StationId:int}/Sensor/{SensorId:int}/DataTimestamp/Filter")]
            //Filter: Hours, Months, Days, Weeks
            public Task<string> GetDatasByStationSensorTimestampHour([FromRoute]int StationId,[FromRoute] int SensorId, 
            [FromQuery] long StartTimestamp, [FromQuery] long EndTimestamp, [FromQuery] string Filter, [FromQuery] int FilterValue)
            {
                return this.GetDataByStationSensorTimeStampFilter(StationId, SensorId, StartTimestamp, EndTimestamp, Filter, FilterValue);
            }

            private async Task<string> GetDataByStationSensorTimeStampFilter(int StationId, int SensorId, 
            long StartTimestamp, long EndTimeStamp, String Filter, int FilterValue)
            {
                var Datas= await _DataRepository.GetByStationSensorTimestampFilter(StationId, SensorId, 
                StartTimestamp, EndTimeStamp, Filter, FilterValue);
                return JsonConvert.SerializeObject(Datas);
            }



            [HttpGet]
            [Route("api/Station/{StationId:int}/Sensor/{SensorId:int}/[controller]/{DataId:int}")]
            public Task<string> Get([FromRoute]int StationId, [FromRoute]int SensorId, [FromRoute]int DataId)
            {
                return this.GetDataById(StationId, SensorId, DataId);
            }

            private async Task<string> GetDataById(int StationId, int SensorId, int DataId)
            {
                var Data= await _DataRepository.Get(StationId, SensorId, DataId) ?? new Data();
                return JsonConvert.SerializeObject(Data);
            }
            
            [HttpPost]
            [Route("api/[controller]")]
            public string Post([FromBody] Arrays Array)
            {
                List<Data> data=Array.Data;
                for (var i = 0; i <data.Count; i++) {
                    var sensorId=data[i].SensorId;
                    var stationId=data[i].StationId;
                    Sensor Sensor= _SensorRepository.getSensorByStation(stationId, sensorId);
                    if(Sensor==null){
                        var type=data[i].Type;
                        var location=data[i].Location;
                        var newSensor=new Sensor();
                        newSensor.Id=sensorId;
                        newSensor.Type=type;
                        newSensor.Location=location;
                        newSensor.StationId=stationId;
                        _SensorRepository.Add(newSensor);
                    }
                    _DataRepository.Add(data[i]);
                }
                /* 
                await _DataRepository.Add(Data);
                */
                return "";
            }

            [HttpPut]
            [Route("api/[controller]/{id}")]
            public async Task<bool> Put(string id, [FromBody] Data Data)
            {
                if (string.IsNullOrEmpty(id)) return false;
                return await _DataRepository.Update(id, Data);
            }

            [HttpDelete]
            [Route("api/[controller]/{id}")]
            public async Task<bool> Delete(string id)
            {
                if (string.IsNullOrEmpty(id)) return false;
                return await _DataRepository.Remove(id);
            }
            
            [HttpGet]
            [Route("api/Station/{StationId:int}/AvgPerDate")]        
            public Task<string> GetAvgPerDate(
                [FromRoute]int StationId,
                [FromQuery] long StartTimestamp,
                [FromQuery] long EndTimestamp
            )
            {
                return this._GetAvgPerDate(
                    StationId, 
                    StartTimestamp, 
                    EndTimestamp
                );
            }

            private async Task<string> _GetAvgPerDate(
                int StationId,
                long StartTimestamp,
                long EndTimestamp
            )
            {
                var data = await _DataRepository.AvgPerDate(
                    StationId, 
                    StartTimestamp, 
                    EndTimestamp
                );
                return data.ToJson() ;
            }

            [HttpGet]
            [Route("api/Station/{StationId:int}/AvgPerHour")]        
            public Task<string> GetAvgPerHour(
                [FromRoute]int StationId,
                [FromQuery] long StartTimestamp,
                [FromQuery] long EndTimestamp
            )
            {
                return this._GetAvgPerHour(
                    StationId, 
                    StartTimestamp, 
                    EndTimestamp
                );
            }

            private async Task<string> _GetAvgPerHour(
                int StationId,
                long StartTimestamp,
                long EndTimestamp
            )
            {
                var data = await _DataRepository.AvgPerHour(
                    StationId, 
                    StartTimestamp, 
                    EndTimestamp
                );
                return data.ToJson() ;
            }

            [HttpGet]
            [Route("api/Station/{StationId:int}/AvgPerMonth")]        
            public Task<string> GetAvgPerMonth(
                [FromRoute]int StationId,
                [FromQuery] long StartTimestamp,
                [FromQuery] long EndTimestamp
            )
            {
                return this._GetAvgPerMonth(
                    StationId, 
                    StartTimestamp, 
                    EndTimestamp
                );
            }

            private async Task<string> _GetAvgPerMonth(
                int StationId,
                long StartTimestamp,
                long EndTimestamp
            )
            {
                var data = await _DataRepository.AvgPerMonth(
                    StationId, 
                    StartTimestamp, 
                    EndTimestamp
                );
                return data.ToJson() ;
            }

            [HttpGet]
            [Route("api/AvgPerDateStation")]
            public Task<string> GetAvgPerDateStation(            
                [FromQuery] string SensorType,
                [FromQuery] string SensorLocation,
                [FromQuery] long StartTimestamp,
                [FromQuery] long EndTimestamp
            )
            {
                return this._GetAvgPerDateStation(
                    SensorType,
                    SensorLocation,
                    StartTimestamp, 
                    EndTimestamp
                );
            }

            private async Task<string> _GetAvgPerDateStation(
                string SensorType,
                string SensorLocation,
                long StartTimestamp,
                long EndTimestamp
            )
            {
                var data = await _DataRepository.AvgPerDateStation(
                    SensorType,
                    SensorLocation,
                    StartTimestamp, 
                    EndTimestamp
                );
                return data.ToJson() ;
            }

        }
}