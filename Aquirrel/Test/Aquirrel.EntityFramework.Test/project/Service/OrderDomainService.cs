using Aquirrel.EntityFramework.Test.project.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.EntityFramework.Test.project.Service
{
    public class OrderDomainService
    {
        IOrderRepo orderRepo;
        public OrderDomainService(IOrderRepo orderRepo)
        {
            this.orderRepo = orderRepo;
        }

        public void CancelOrder(string orderId)
        {
            var order = this.orderRepo.Find(orderId);
            order.OrderStatus = 999;

            //DistributedCommitService.Commit(this.orderRepo);
        }
    }
}
