using LockrFront.Models;
using System.Threading.Tasks;

namespace LockrFront.Database
{
    public interface IApiKeyQueries
    {
        /// <summary>
        /// Retrieve the ApiKey
        /// </summary>
        /// <param name="Id">UserId, Subject Id (Identity server identity)</param>
        /// <returns></returns>
        ApiKeyModel RetrieveApiKey(string Id);

        /// <summary>
        /// Save the generated api key (Guid)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task SaveApiKeyAsync(ApiKeyModel model);

        /// <summary>
        /// Update the Api Key
        /// </summary>
        /// <param name="model"></param>
        Task UpdateApiKeyAsync(ApiKeyModel model);
    }
}
