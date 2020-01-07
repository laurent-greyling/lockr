using LockrFront.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LockrFront.Database
{
    /// <summary>
    /// Database context for EF Core 
    /// If you run add-migration YourMigrationName it will create migrations for you
    /// There after run update-database –verbose to create the tables in SQL Server
    /// </summary>
    public class DatabaseContext : DbContext
    {
        public DbSet<ApiKeyEntity> ApiKeyEntity { get; set; }

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
