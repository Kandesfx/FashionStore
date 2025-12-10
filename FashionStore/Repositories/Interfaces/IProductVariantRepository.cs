using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IProductVariantRepository : IRepository<ProductVariant>
    {
        IEnumerable<ProductVariant> GetByProductId(int productId);
        void RemoveByProductId(int productId);
    }
}

