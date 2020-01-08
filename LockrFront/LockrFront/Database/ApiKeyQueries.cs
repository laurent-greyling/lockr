using LockrFront.Entities;
using LockrFront.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                Id = apiKeyEntity == null ? string.Empty : apiKeyEntity.Id
            };
        }

        public async Task SaveApiKeyAsync(ApiKeyModel model)
        {
            _dbContext.ApiKeyEntity.Add(new ApiKeyEntity
            {
                Id = model.Id,
                ApiKey = GenerateApiKey(model.ApiKey)
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateApiKey(ApiKeyModel model)
        {
            _dbContext.ApiKeyEntity.Update(new ApiKeyEntity
            {
                Id = model.Id,
                ApiKey = GenerateApiKey(model.ApiKey)
            });

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Generate a hashed key with userid as identifiable component in key
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GenerateApiKey(string apiKey)
        {
            var key = apiKey.Split(".");
            var hash = SHA256.Create();
            var hashByte = hash.ComputeHash(Encoding.UTF8.GetBytes(key[0]));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashByte)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
