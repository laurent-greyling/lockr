using LockrApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LockrApi.Database
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DbSet<DomainEntity> DomainEntity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var connectionString = configuration["DbConnectionString"];
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
