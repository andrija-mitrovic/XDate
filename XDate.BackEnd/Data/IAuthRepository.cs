using System.Threading.Tasks;
using XDate.BackEnd.Models;

namespace XDate.BackEnd.Data
{
    public interface IAuthRepository
    {
        Task<User> Login(string username,string password);
        Task<User> Register(User user, string password);
        Task<bool> UserExists(string username);
    }
}