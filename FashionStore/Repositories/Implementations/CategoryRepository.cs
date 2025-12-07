using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;

namespace FashionStore.Repositories.Implementations
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

