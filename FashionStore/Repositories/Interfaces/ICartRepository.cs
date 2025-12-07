using FashionStore.Models.Entities;

namespace FashionStore.Repositories.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Cart GetByUserId(int userId);
    }
}

