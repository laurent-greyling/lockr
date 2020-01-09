using LockrApi.Entities;
using System.Threading.Tasks;

namespace LockrApi.Services
{
    /// <summary>
    /// Interface to retrieve spf and verify if it is valid
    /// </summary>
    public interface IVerifyDomain
    {
        /// <summary>
        /// Verify a domain validity and return the entity to be saved
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        Task<DomainEntity> Verify(string domain);
    }
}
