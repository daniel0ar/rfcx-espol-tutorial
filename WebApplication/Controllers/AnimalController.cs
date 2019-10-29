using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApplication.Models;
using WebApplication.ViewModel;
using X.PagedList;



namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AnimalController : Controller
    {
        private readonly IAnimalRepository _AnimalRepository;

        public AnimalController(IAnimalRepository AnimalRepository)
        {
            _AnimalRepository = AnimalRepository;
        }

        [HttpGet("list")]
        public Task<string> Get()
        {
            return this.GetAnimals();
        }

        private async Task<string> GetAnimals()
        {
            var animals = await _AnimalRepository.Get();
            return JsonConvert.SerializeObject(animals);
        }

        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetAnimal(id);
        }

        private async Task<string> GetAnimal(string id)
        {
            var animal = await _AnimalRepository.Get(id) ?? new Animal();
            return JsonConvert.SerializeObject(animal);
        }

        [HttpPost]
        public IActionResult Post()
        {
            return Redirect("index");
        }

        [HttpPut]
        public IActionResult Put()
        {
            return Redirect("index");
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            bool result = await _AnimalRepository.Remove(id);
            return result;
        }

        [HttpGet("index")]
        public IActionResult Index(AnimalViewModel animalVM)
        {
            var pageNumber = (animalVM.Pnumber == 0) ? 1 : animalVM.Pnumber;
            var pageSize = 10;
            var animals = _AnimalRepository.GetAll().ToPagedList(pageNumber, pageSize);
            animalVM.Animals = animals;
            return View(animalVM);
        }
    }
}
