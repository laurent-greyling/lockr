using LockrFront.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace LockrFront.Database
{
    public interface IDatabaseContext
    {
        DbSet<ApiKeyEntity> ApiKeyEntity { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
