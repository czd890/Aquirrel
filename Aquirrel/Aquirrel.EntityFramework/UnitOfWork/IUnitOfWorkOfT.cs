using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext {
        TContext DbContext { get; }

        Task<int> SaveChangesAsync(bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks);
    }
}
