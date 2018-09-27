using Aquirrel.EntityFramework.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    /// <summary>
    /// client程序端分布式2阶段提交方案实现服务
    /// </summary>
    public interface IDistributedCommitService
    {
        int SaveChanges(params IPersistence[] persistence);
        Task<int> SaveChangesAsync(params IPersistence[] persistence);
        Task<int> SaveChangesAsync(IPersistence[] persistence, CancellationToken cancellationToken = default(CancellationToken));
    }
    public class DistributedCommitService : IDistributedCommitService
    {
        public int SaveChanges(params IPersistence[] persistence)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(params IPersistence[] persistence)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(IPersistence[] persistence, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
