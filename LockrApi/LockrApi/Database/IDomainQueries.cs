using LockrApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LockrApi.Database
{
    public interface IDomainQueries
    {
        /// <summary>
        /// Retrieve the Domain details for the supplied domain/email
        /// </summary>
        /// <param name="domainName">UserId, Subject Id (Identity server identity)</param>
        /// <returns></returns>
        List<DomainEntity> RetrieveDomainDetails();

        /// <summary>
        /// Save the domain details for the spf record of the domain
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task SaveApiKeyAsync(DomainEntity entity);
    }
}
