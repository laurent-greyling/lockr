using System.Threading.Tasks;

namespace LockrFront.Services
{
    /// <summary>
    /// Request Token from Identity server
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Request the bearer token from Identity server in order to access the api from client
        /// </summary>
        /// <returns></returns>
        Task<string> RequestAcccessTokenAsync();
    }
}
