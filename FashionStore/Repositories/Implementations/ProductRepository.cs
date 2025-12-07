using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Product> GetFeaturedProducts()
        {
            return _dbSet
                .Where(p => p.Featured && p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .ToList();
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _dbSet
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToList();
        }

        public IEnumerable<Product> GetProductsWithCategory()
        {
            return _dbSet
                .Include("Category")
                .Where(p => p.IsActive)
                .ToList();
        }
    }
}

