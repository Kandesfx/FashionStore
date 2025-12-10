using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ProductVariant> GetByProductId(int productId)
        {
            return _dbSet.Where(v => v.ProductId == productId).ToList();
        }

        public void RemoveByProductId(int productId)
        {
            var items = _dbSet.Where(v => v.ProductId == productId).ToList();
            _dbSet.RemoveRange(items);
        }
    }
}

