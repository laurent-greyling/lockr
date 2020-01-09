using LockrApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LockrApi.Database
{
    public class DomainQueries : IDomainQueries
    {
        public IDatabaseContext _dbContext;

        public DomainQueries(IDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DomainEntity> RetrieveDomainDetails()
        {
            return  _dbContext.DomainEntity.ToList();
        }

        public async Task SaveApiKeyAsync(DomainEntity entity)
        {
            _dbContext.DomainEntity.Add(entity);

            await _dbContext.SaveChangesAsync();
        }
    }
}
