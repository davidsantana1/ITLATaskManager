using System.Security.Cryptography;
using System.Text;

namespace ITLATaskManagerAPI.Security
{
    public interface IAuthService
    {
        Task<string> GetToken(string email, string password);
    }
}
