using LockrApi.Entities;
using LockrApi.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LockrApi.Database
{
    public class DomainQueries : IDomainQueries
    {
        public IDatabaseContext _dbContext;
        public IDomainValidation _domainValidation;

        public DomainQueries(
            IDatabaseContext dbContext,
            IDomainValidation domainValidation)
        {
            _dbContext = dbContext;
            _domainValidation = domainValidation;
        }

        public List<DomainEntity> RetrieveDomainDetails()
        {
            return  _dbContext.DomainEntity.ToList();
        }

        public async Task<DomainEntity> RetrieveDomainDetailsAsync(string Id)
        {
            var domainId = Id;

            if (_domainValidation.IsValidEmail(Id))
            {
                domainId = _domainValidation.DomainFromEmail(Id);
            }

            return await _dbContext.DomainEntity.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == domainId);
        }

        public async Task SaveDomainDetailsAsync(DomainEntity entity)
        {
            _dbContext.DomainEntity.Add(entity);

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateDomainDetailsAsync(DomainEntity entity)
        {
            _dbContext.DomainEntity.Update(entity);

            await _dbContext.SaveChangesAsync();        
        }
    }
}
