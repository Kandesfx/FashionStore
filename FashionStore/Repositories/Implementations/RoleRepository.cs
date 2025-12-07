using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;

namespace FashionStore.Repositories.Implementations
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

