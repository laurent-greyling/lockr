using LockrFront.Database;
using LockrFront.Entities;
using LockrFront.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace LockrFrontTests
{
    public class Tests
    {
        public Mock<DbSet<ApiKeyEntity>> _mockApiKeyEntity;
        public Mock<IDatabaseContext> _mockDbContext;
        public Mock<IApiKeyQueries> _mockApiKeyQueries;

        public string UserId = "User1";
        public string ApiKey = "ApiKey1";

        public ApiKeyModel Model;
        public ApiKeyQueries Result;

        [SetUp]
        public void Setup()
        {
            Model = new ApiKeyModel
            {
                Id = UserId,
                ApiKey = ApiKey
            };

            _mockApiKeyEntity = new Mock<DbSet<ApiKeyEntity>>();
            _mockDbContext = new Mock<IDatabaseContext>();
            _mockApiKeyQueries = new Mock<IApiKeyQueries>();

            _mockDbContext.Setup(a => a.ApiKeyEntity).Returns(_mockApiKeyEntity.Object);
            _mockApiKeyQueries.Setup(x => x.RetrieveApiKey(UserId)).Returns(Model);

            Result = new ApiKeyQueries(_mockDbContext.Object);
        }

        [Test]
        public async Task Test_SaveApiKeyAsync()
        {
            await Result.SaveApiKeyAsync(Model);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Test_UpdateApiKeyAsync_TimesOnce()
        {
            await Result.UpdateApiKeyAsync(Model);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Test_RetrieveApiKey()
        {
            var apiKeyModel = _mockApiKeyQueries.Object.RetrieveApiKey(UserId);

            Assert.That(apiKeyModel == Model);
        }
    }
}