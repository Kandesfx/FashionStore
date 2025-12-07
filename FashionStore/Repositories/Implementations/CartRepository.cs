using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Cart GetByUserId(int userId)
        {
            return _dbSet.FirstOrDefault(c => c.UserId == userId);
        }
    }
}

