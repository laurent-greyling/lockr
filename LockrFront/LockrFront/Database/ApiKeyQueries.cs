using LockrFront.Entities;
using LockrFront.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LockrFront.Database
{
    public class ApiKeyQueries : IApiKeyQueries
    {
        public IDatabaseContext _dbContext;

        public ApiKeyQueries(IDatabaseContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public ApiKeyModel RetrieveApiKey(string Id)
        {
            var apiKeyEntity = _dbContext.ApiKeyEntity.AsNoTracking().FirstOrDefault(i=> i.Id == Id);

            return new ApiKeyModel 
            {
                Id = Id,
                ApiKey = apiKeyEntity == null ? string.Empty : apiKeyEntity.ApiKey 
            };
        }

        public async Task SaveApiKeyAsync(ApiKeyModel model)
        {
            _dbContext.ApiKeyEntity.Add(new ApiKeyEntity
            {
                Id = model.Id,
                ApiKey = model.ApiKey
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateApiKey(ApiKeyModel model)
        {
            _dbContext.ApiKeyEntity.Update(new ApiKeyEntity
            {
                Id = model.Id,
                ApiKey = model.ApiKey
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
