using System.Collections.Generic;
using System.Threading.Tasks;
using XDate.BackEnd.Helpers;
using XDate.BackEnd.Models;

namespace XDate.BackEnd.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T Entity) where T: class;
         void Delete<T>(T Entity) where T: class;
         Task<PagedList<User>> GetUsers(UserParams userParams);
         Task<User> GetUser(int id);
         Task<bool> SaveAll();
         Task<Photo> GetPhoto(int id);
         Task<Photo> GetCurrentPhoto(int userId);
    }
}