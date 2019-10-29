using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class LabelController
    {
        
        private readonly ILabelRepository _LabelRepository;

        public LabelController(ILabelRepository LabelRepository)
        {
            _LabelRepository=LabelRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetLabel();
        }

        public async Task<string> GetLabel()
        {
            var Labels= await _LabelRepository.Get();
            return JsonConvert.SerializeObject(Labels);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetLabelById(id);
        }

        public async Task<string> GetLabelById(string id)
        {
            var Label= await _LabelRepository.Get(id) ?? new Label();
            return JsonConvert.SerializeObject(Label);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Label Label)
        {
            await _LabelRepository.Add(Label);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Label Label)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _LabelRepository.Update(id, Label);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _LabelRepository.Remove(id);
        }
    }
}