using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public IEnumerable<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public IEnumerable<Product> GetActiveProducts()
        {
            return _productRepository.Find(p => p.IsActive);
        }

        public void Add(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.ProductName))
                throw new ArgumentException("Tên sản phẩm không được để trống");

            if (product.Price < 0)
                throw new ArgumentException("Giá sản phẩm phải >= 0");

            _productRepository.Add(product);
            _unitOfWork.Complete();
        }

        public void Update(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var existingProduct = _productRepository.GetById(product.Id);
            if (existingProduct == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại");

            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.DiscountPrice = product.DiscountPrice;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.Stock = product.Stock;
            existingProduct.Featured = product.Featured;
            existingProduct.IsActive = product.IsActive;

            _productRepository.Update(existingProduct);
            _unitOfWork.Complete();
        }

        public void Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại");

            // Soft delete
            product.IsActive = false;
            _productRepository.Update(product);
            _unitOfWork.Complete();
        }

        public IEnumerable<Product> GetFeaturedProducts(int count = 8)
        {
            return _productRepository
                .Find(p => p.Featured && p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .Take(count);
        }

        public IEnumerable<Product> GetLatestProducts(int count = 8)
        {
            return _productRepository
                .Find(p => p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .Take(count);
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _productRepository
                .Find(p => p.CategoryId == categoryId && p.IsActive);
        }

        public IEnumerable<Product> SearchProducts(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return GetActiveProducts();

                // Normalize từ khóa tìm kiếm: trim và lowercase
                var searchTermNormalized = (searchTerm ?? string.Empty).Trim().ToLowerInvariant();
                
                if (string.IsNullOrWhiteSpace(searchTermNormalized))
                    return GetActiveProducts();
                
                // Load tất cả sản phẩm active vào memory để đảm bảo ToLower() hoạt động đúng
                var allActiveProducts = _productRepository.Find(p => p.IsActive).ToList();
                
                // Filter với cả ProductName và searchTerm đều lowercase
                var results = allActiveProducts
                    .Where(p => 
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(p?.ProductName))
                                return false;
                            
                            // Normalize ProductName: trim và lowercase
                            var productNameNormalized = p.ProductName.Trim().ToLowerInvariant();
                            
                            // Kiểm tra ProductName chứa searchTerm
                            if (productNameNormalized.Contains(searchTermNormalized))
                                return true;
                            
                            // Kiểm tra Description nếu có
                            if (!string.IsNullOrWhiteSpace(p.Description))
                            {
                                var descriptionNormalized = p.Description.Trim().ToLowerInvariant();
                                if (descriptionNormalized.Contains(searchTermNormalized))
                                    return true;
                            }
                            
                            return false;
                        }
                        catch
                        {
                            return false;
                        }
                    })
                    .ToList();
                
                return results;
            }
            catch (Exception)
            {
                // Nếu có lỗi, trả về danh sách rỗng thay vì throw exception
                return new List<Product>();
            }
        }

        public IEnumerable<Product> GetProductsWithPaging(int page, int pageSize, out int totalCount)
        {
            var products = GetActiveProducts();
            totalCount = products.Count();
            
            return products
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public IEnumerable<Product> FilterProducts(int? categoryId, decimal? minPrice, decimal? maxPrice, bool? featured)
        {
            var query = _productRepository.Find(p => p.IsActive).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.FinalPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.FinalPrice <= maxPrice.Value);

            if (featured.HasValue)
                query = query.Where(p => p.Featured == featured.Value);

            return query.ToList();
        }

        public bool CheckStock(int productId, int quantity)
        {
            var product = GetById(productId);
            return product != null && product.Stock >= quantity;
        }

        public void UpdateStock(int productId, int quantity)
        {
            var product = GetById(productId);
            if (product == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại");

            if (product.Stock < quantity)
                throw new InvalidOperationException("Số lượng tồn kho không đủ");

            product.Stock -= quantity;
            _productRepository.Update(product);
            _unitOfWork.Complete();
        }
    }
}

