using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public User GetByUsername(string username)
        {
            return _dbSet.FirstOrDefault(u => u.Username == username);
        }

        public User GetByEmail(string email)
        {
            return _dbSet.FirstOrDefault(u => u.Email == email);
        }
    }
}

