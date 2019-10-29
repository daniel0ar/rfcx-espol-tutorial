using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User Get(string userName);
        User GetById(string userId);
        User AddAsync(User item);
        bool Update(string userId, User item);
        bool UpdateById(string userId, User item);
        bool Remove(string userId);
        /* 
        Task<User> Get(string id);
        bool Update(int id, Question item);
        bool UpdateSpecieId(int id, int specie_id);
        bool UpdateText(int id, string text);
        bool UpdateOption(int id, int index, string option);
        bool UpdateAnswer(int id, int answer);
        bool UpdateFeedback(int id, string feedback);
        bool UpdateCategory(int id, string category);
        bool UpdateStations(int id, List<Station> stations);
        bool Remove(int id);
        */
    }

}