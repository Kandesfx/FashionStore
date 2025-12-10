using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override User GetById(int id)
        {
            return _dbSet.Include("Role").FirstOrDefault(u => u.Id == id);
        }

        public User GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;
            
            // Phân biệt hoa thường - so sánh chính xác
            return _dbSet.Include("Role").FirstOrDefault(u => u.Username == username);
        }

        public User GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            
            // Phân biệt hoa thường - so sánh chính xác
            return _dbSet.Include("Role").FirstOrDefault(u => u.Email == email);
        }
    }
}

