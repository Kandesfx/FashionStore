using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        IEnumerable<CartItem> GetCartItemsWithProduct(int cartId);
    }
}

