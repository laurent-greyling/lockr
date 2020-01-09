using LockrApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace LockrApi.Database
{
    /// <summary>
    /// Creates database context in order for us to query our tables
    /// </summary>
    public interface IDatabaseContext
    {
        public DbSet<DomainEntity> DomainEntity { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
