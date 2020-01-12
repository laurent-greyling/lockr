using LockrApi.Database;
using LockrApi.Models;
using LockrApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LockrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DomainController : ControllerBase
    {
        public IVerifyDomain _verifyDomain;
        public IDomainQueries _domainQueries;

        /// <summary>
        /// Controller used to verify a domain and save it. 
        /// After domain has been saved you can retrieve a list of domains and their properties.
        /// </summary>
        /// <param name="verifyDomain"></param>
        /// <param name="domainQueries"></param>
        public DomainController(
            IVerifyDomain verifyDomain,
            IDomainQueries domainQueries)
        {
            _verifyDomain = verifyDomain;
            _domainQueries = domainQueries;
        }

        /// <summary>
        /// Get the domain entities
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DomainModel> Get()
        {
            var domainEntities = _domainQueries.RetrieveDomainDetails();
            return  domainEntities.Select(x =>
            {
                return new DomainModel 
                {
                    Id = x.Id,
                    Version = x.Version,
                    ExpiryData = x.ExpiryData,
                    Provider = x.Provider,
                    NtaMxList = x.NtaMxList,
                    SpfVersion = x.SpfVersion,
                    IsValid = x.IsValid
                };
            }).ToList();
        }

        /// <summary>
        /// Verify and save domains
        /// Valid and invalid domains are saved with their status as valid or invalid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Post(DomainModel model)
        {
            var domainEntity = await _verifyDomain.Verify(model.Id);

            if (domainEntity == null)
            {
                return;
            }

            var entity = await _domainQueries.RetrieveDomainDetailsAsync(model.Id);
            //If we have the same entity update
            if (entity != null)
            {
                await _domainQueries.UpdateDomainDetailsAsync(domainEntity);
                return;
            }

            //Will only save domains that are validated as a domain.
            //Will still save even if invalid spf domain, but will ignore any string that was not parsed as a domain or email
            await _domainQueries.SaveDomainDetailsAsync(domainEntity);           
        }
    }
}