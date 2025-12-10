using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface IProductVariantService
    {
        IEnumerable<ProductVariant> GetByProductId(int productId);
        void ReplaceVariants(int productId, IEnumerable<ProductVariant> variants);
    }
}

