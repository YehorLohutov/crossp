using System.Threading.Tasks;
using MAUI.Models;

namespace MAUI.Services
{
    public interface IAuthorizationService
    {
        Task<Token> GetTokenAsync(string login, string password);
    }
}
