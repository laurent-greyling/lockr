using LockrApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LockrApi.Database
{
    public interface IDomainQueries
    {
        /// <summary>
        /// Retrieve the Domain details
        /// </summary>
        /// <param name="domainName">UserId, Subject Id (Identity server identity)</param>
        /// <returns></returns>
        List<DomainEntity> RetrieveDomainDetails();

        /// <summary>
        /// Retrieve the Domain details for selected Domain
        /// </summary>
        /// <param name="domainName">UserId, Subject Id (Identity server identity)</param>
        /// <returns></returns>
        Task<DomainEntity> RetrieveDomainDetailsAsync(string Id);

        /// <summary>
        /// Save the domain details for the spf record of the domain
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task SaveDomainDetailsAsync(DomainEntity entity);

        /// <summary>
        /// Update the domain details for the spf record of the domain
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateDomainDetailsAsync(DomainEntity entity);
    }
}
