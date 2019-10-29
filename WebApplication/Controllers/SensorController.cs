using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;


namespace WebApplication.Controllers
{
    [Route("")]
    public class SensorController
    {
        
        private readonly ISensorRepository _SensorRepository;

        public SensorController(ISensorRepository SensorRepository)
        {
            _SensorRepository=SensorRepository;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public Task<string> Get()
        {
            return this.GetSensor();
        }

        private async Task<string> GetSensor()
        {
            var Sensors= await _SensorRepository.Get();
            return JsonConvert.SerializeObject(Sensors);
        }

        /* */
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public Task<string> Get(string id)
        {
            return this.IdString(id);
        }

        private async Task<string> IdString(string id)
        {
            var Sensor= await _SensorRepository.Get(id) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }
        
        [HttpGet]
        [Route("api/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.IntId(id);
        }

        private async Task<string> IntId(int id)
        {
            var Sensor= await _SensorRepository.Get(id) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }

        [HttpGet]
        [Route("api/Station/{StationId:int}/[controller]")]
        public Task<string> GetSensorsByStation([FromRoute]int StationId)
        {
            return this.GetSensorByStation(StationId);
        }

        private async Task<string> GetSensorByStation(int StationId)
        {
            var Sensors= await _SensorRepository.GetByStation(StationId);
            return JsonConvert.SerializeObject(Sensors);
        }


        [HttpGet]
        [Route("api/Station/{StationId:int}/[controller]/{SensorId:int}")]
        public Task<string> Get([FromRoute]int StationId, [FromRoute]int SensorId)
        {
            return this.GetSensorById(StationId,SensorId);
        }

        private async Task<string> GetSensorById(int StationId, int SensorId)
        {
            var Sensor= await _SensorRepository.Get(StationId, SensorId) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }

        /*
        [HttpPost]
        [Route("api/[controller]")]
        public async Task<string> Post([FromBody] Sensor Sensor)
        {
            await _SensorRepository.Add(Sensor);
            return "";
        }
        */

        [HttpPut]
        [Route("api/[controller]/{id}")]
        public async Task<bool> Put(string id, [FromBody] Sensor Sensor)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _SensorRepository.Update(id, Sensor);
        }

        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _SensorRepository.Remove(id);
        }
    }
}