using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;

using System.Collections.Generic;



namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class InfoSensoresController
    {
        
        private readonly IInfoSensoresRepository _InfoSensoresRepository;
        private readonly IDataRepository _DataRepository;

        public InfoSensoresController(IInfoSensoresRepository InfoSensoresRepository, IDataRepository DataRepository)
        {
            _InfoSensoresRepository=InfoSensoresRepository;
            _DataRepository=DataRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetInfoSensores();
        }

        public async Task<string> GetInfoSensores()
        {
            var InfoSensoress= await _InfoSensoresRepository.Get();
            return JsonConvert.SerializeObject(InfoSensoress);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetInfoSensoresById(id);
        }

        public async Task<string> GetInfoSensoresById(string id)
        {
            var InfoSensores= await _InfoSensoresRepository.Get(id) ?? new InfoSensores();
            return JsonConvert.SerializeObject(InfoSensores);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] InfoSensores InfoSensores)
        {
            List<Data> DataList = InfoSensores.Data;
            for (var i = 0; i < DataList.Count; i++) {
                _DataRepository.Add(DataList[i]);
            }
            await _InfoSensoresRepository.Add(InfoSensores);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] InfoSensores InfoSensores)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _InfoSensoresRepository.Update(id, InfoSensores);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _InfoSensoresRepository.Remove(id);
        }
    }
}