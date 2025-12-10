using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetFeaturedProducts();
        IEnumerable<Product> GetProductsByCategory(int categoryId);
        IEnumerable<Product> GetProductsWithCategory();
    }
}

