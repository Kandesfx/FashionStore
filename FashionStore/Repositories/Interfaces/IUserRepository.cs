using FashionStore.Models.Entities;

namespace FashionStore.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByUsername(string username);
        User GetByEmail(string email);
    }
}

