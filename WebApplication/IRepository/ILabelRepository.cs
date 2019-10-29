using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface ILabelRepository
    {
        Task<IEnumerable<Label>> Get();
        Task<Label> Get(string id);
        Task Add(Label item);
        Task<bool> Update(string id, Label item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}