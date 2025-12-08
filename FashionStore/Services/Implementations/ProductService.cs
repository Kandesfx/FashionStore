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
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetActiveProducts();

            var term = searchTerm.ToLower();
            return _productRepository
                .Find(p => p.IsActive && 
                    (p.ProductName.ToLower().Contains(term) || 
                     (p.Description != null && p.Description.ToLower().Contains(term))));
        }

        public IEnumerable<Product> SearchProductsLive(string searchTerm, int limit = 5)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Product>();

            if (_productRepository == null)
                return Enumerable.Empty<Product>();

            var term = searchTerm.ToLower().Trim();
            
            try
            {
                // Get all active products first, then filter in memory to avoid LINQ translation issues
                var allActiveProducts = _productRepository.Find(p => p.IsActive);
                return allActiveProducts
                    .Where(p => p.ProductName != null && p.ProductName.ToLower().Contains(term))
                    .Take(limit)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchProductsLive: {ex.Message}");
                return Enumerable.Empty<Product>();
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

