using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductVariantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ProductVariant> GetByProductId(int productId)
        {
            return _unitOfWork.ProductVariants.GetByProductId(productId);
        }

        public void ReplaceVariants(int productId, IEnumerable<ProductVariant> variants)
        {
            // remove old
            _unitOfWork.ProductVariants.RemoveByProductId(productId);
            // add new
            if (variants != null && variants.Any())
            {
                foreach (var v in variants)
                {
                    v.ProductId = productId;
                    _unitOfWork.ProductVariants.Add(v);
                }
            }
            _unitOfWork.Complete();
        }
    }
}

