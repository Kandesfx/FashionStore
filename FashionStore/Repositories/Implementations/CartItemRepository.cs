using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<CartItem> GetCartItemsWithProduct(int cartId)
        {
            return _dbSet
                .Include("Product")
                .Include("ProductVariant")
                .Where(ci => ci.CartId == cartId)
                .ToList();
        }
    }
}

