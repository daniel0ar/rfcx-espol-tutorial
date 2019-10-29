using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;
using System.Linq;

namespace WebApplication.IRepository
{
    public interface IAnimalRepository
    {
        IQueryable<Animal> GetAll();
        Task<IEnumerable<Animal>> Get();
        Task<Animal> Get(string id);
        Task<bool> Add(Animal item);
        Task<bool> Update(string id, Animal item);
        Task<bool> Remove(string id);
    }
}