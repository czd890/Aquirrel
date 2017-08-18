using Aquirrel.EntityFramework.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.EntityFramework
{
    public static class DistributedCommitService
    {
        public static bool Commit(params object[] repo)
        {
            List<IDbContextTransaction> trans = new List<IDbContextTransaction>();
            //foreach (var item in repo)
            //{
            //    (item as IRepository)?.DbConext.Database.BeginTransaction()
            //}
            return false;
        }
    }
}
