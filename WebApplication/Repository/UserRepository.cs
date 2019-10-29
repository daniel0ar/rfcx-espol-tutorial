using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace WebApplication.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ObjectContext _context =null; 

        private readonly Regex rgx = new Regex(@"^(?=.*?[a-z])(?=.*?[0-9]).{8,}$");

        public UserRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public List<User> GetAll()
        {
            try
            {
                return _context.Users.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User Get(string Username)
        {
            var filter = Builders<User>.Filter.Eq("username", Username);

            try
            {
                return _context.Users.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User GetById(string userId)
        {
            var filter = Builders<User>.Filter.Eq("UserId", userId);

            try
            {
                return _context.Users.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User AddAsync(User item)
        {
            try
            {
                var userExists =  _context.Users.Find(u => u.Username == item.Username).FirstOrDefault();
                if (userExists == null) 
                {
                    if(rgx.IsMatch(item.Password))
                    {
                        _context.Users.InsertOne(item);
                        return item;
                    }
                }
                return null;
                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Write(ex.Source);
                Console.Write(ex.StackTrace);
                return null;
            }
        }

        public bool Update(string userName, User item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = _context.Users
                                    .ReplaceOne(n => n.Username.Equals(userName)
                                            , item
                                            , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateById(string userId, User item)
        {
            try
            {
                ReplaceOneResult actionResult
                    = _context.Users
                                    .ReplaceOne(n => n.UserId.Equals(userId)
                                            , item
                                            , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Remove(string userId)
        {
            try
            {
                DeleteResult actionResult = _context.Users.DeleteOne(user => user.UserId == userId); 
                return actionResult.IsAcknowledged
                        && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Write("error: " + ex.StackTrace + "\n");
                Console.Write("error: " + ex.Message + "\n");
                throw ex;
            }
        }
    }
}