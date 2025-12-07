using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public Category GetById(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public IEnumerable<Category> GetAll()
        {
            return _categoryRepository.GetAll();
        }

        public IEnumerable<Category> GetActiveCategories()
        {
            return _categoryRepository.Find(c => c.IsActive);
        }

        public void Add(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Tên danh mục không được để trống");

            _categoryRepository.Add(category);
            _unitOfWork.Complete();
        }

        public void Update(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var existingCategory = _categoryRepository.GetById(category.Id);
            if (existingCategory == null)
                throw new InvalidOperationException("Danh mục không tồn tại");

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.Description = category.Description;
            existingCategory.ImageUrl = category.ImageUrl;
            existingCategory.DisplayOrder = category.DisplayOrder;
            existingCategory.IsActive = category.IsActive;

            _categoryRepository.Update(existingCategory);
            _unitOfWork.Complete();
        }

        public void Delete(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
                throw new InvalidOperationException("Danh mục không tồn tại");

            // Check if category has products
            var productCount = GetProductCount(id);
            if (productCount > 0)
                throw new InvalidOperationException("Không thể xóa danh mục đang có sản phẩm");

            // Soft delete
            category.IsActive = false;
            _categoryRepository.Update(category);
            _unitOfWork.Complete();
        }

        public int GetProductCount(int categoryId)
        {
            return _productRepository.Count(p => p.CategoryId == categoryId && p.IsActive);
        }
    }
}

