using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface ICategoryService
    {
        Category GetById(int id);
        IEnumerable<Category> GetAll();
        IEnumerable<Category> GetActiveCategories();
        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
        int GetProductCount(int categoryId);
    }
}

