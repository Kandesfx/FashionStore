using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Order GetById(int id)
        {
            return _dbSet
                .Include(o => o.OrderDetails)
                .Include("OrderDetails.Product")
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);
        }
    }
}

