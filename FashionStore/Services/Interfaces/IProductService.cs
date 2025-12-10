using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface IProductService
    {
        Product GetById(int id);
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetActiveProducts();
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
        
        IEnumerable<Product> GetFeaturedProducts(int count = 8);
        IEnumerable<Product> GetLatestProducts(int count = 8);
        IEnumerable<Product> GetProductsByCategory(int categoryId);
        IEnumerable<Product> SearchProducts(string searchTerm);
        IEnumerable<Product> SearchProductsLive(string searchTerm, int limit = 5);
        IEnumerable<Product> GetProductsWithPaging(int page, int pageSize, out int totalCount);
        IEnumerable<Product> FilterProducts(int? categoryId, decimal? minPrice, decimal? maxPrice, bool? featured);
        
        bool CheckStock(int productId, int quantity);
        void UpdateStock(int productId, int quantity);

        // Variants
        IEnumerable<ProductVariant> GetVariantsByProductId(int productId);
        ProductVariant GetVariantById(int variantId);
    }
}

