using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FashionStore.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Basic CRUD
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        
        // Write Operations
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        
        // Count
        int Count();
        int Count(Expression<Func<T, bool>> predicate);
        
        // Exists
        bool Exists(Expression<Func<T, bool>> predicate);
    }
}

