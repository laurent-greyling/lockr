using LockrFront.Models;
using System.Threading.Tasks;

namespace LockrFront.Services
{
    /// <summary>
    /// Requests to external api
    /// </summary>
    public interface IApiRequest
    {
        /// <summary>
        /// Get information from HttpGet
        /// </summary>
        /// <param name="controller">name of the controller to navigate to</param>
        /// <returns></returns>
        Task<string> GetAsync(string controller);

        /// <summary>
        /// Post information
        /// </summary>
        /// <param name="controller">name of the controller to navigate to</param>
        /// <returns></returns>
        Task PostAsync(DomainModel model, string controller);
    }
}
