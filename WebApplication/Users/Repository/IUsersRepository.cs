using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Models;

namespace Users.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUserBy(int userId);
        Task<User> GetUserBy(string userExternalId);
        Task<User> GetUserBy(string login, string password);
        Task<bool> UserExists(string userExternalId);
        Task Insert(User user);

        void Delete(User user);
        Task Delete(int userId);
        Task Delete(string userExternalId);
        Task SaveChanges();
    }
}
