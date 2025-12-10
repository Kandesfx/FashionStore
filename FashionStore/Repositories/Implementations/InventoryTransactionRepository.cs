using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class InventoryTransactionRepository : Repository<InventoryTransaction>, IInventoryTransactionRepository
    {
        public InventoryTransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<InventoryTransaction> GetByProductId(int productId)
        {
            return _dbSet
                .Include(t => t.Product)
                .Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        public IEnumerable<InventoryTransaction> GetByTransactionType(string transactionType)
        {
            return _dbSet
                .Include(t => t.Product)
                .Where(t => t.TransactionType == transactionType)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        public IEnumerable<InventoryTransaction> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbSet
                .Include(t => t.Product)
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        public IEnumerable<InventoryTransaction> GetByProductAndDateRange(int productId, DateTime startDate, DateTime endDate)
        {
            return _dbSet
                .Include(t => t.Product)
                .Where(t => t.ProductId == productId && t.CreatedDate >= startDate && t.CreatedDate <= endDate)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        public IEnumerable<Product> GetLowStockProducts(int threshold)
        {
            // Ưu tiên kiểm tra tồn theo biến thể; nếu sản phẩm không có biến thể thì dùng tồn sản phẩm
            var products = _context.Set<Product>()
                .Where(p => p.IsActive)
                .ToList();

            var variants = _context.Set<ProductVariant>().Where(v => v.IsActive).ToList();
            var lowStockProducts = new List<Product>();

            foreach (var product in products)
            {
                var productVariants = variants.Where(v => v.ProductId == product.Id).ToList();

                if (productVariants.Any())
                {
                    // Nếu bất kỳ biến thể nào dưới ngưỡng thì cảnh báo cho sản phẩm
                    if (productVariants.Any(v => v.Stock <= threshold))
                    {
                        lowStockProducts.Add(product);
                    }
                }
                else
                {
                    if (product.Stock <= threshold)
                    {
                        lowStockProducts.Add(product);
                    }
                }
            }

            return lowStockProducts;
        }

        public int GetCurrentStock(int productId, int? productVariantId = null)
        {
            if (productVariantId.HasValue)
            {
                var variant = _context.Set<ProductVariant>().FirstOrDefault(v => v.Id == productVariantId.Value);
                if (variant != null)
                {
                    return variant.Stock;
                }
            }

            var product = _context.Set<Product>().FirstOrDefault(p => p.Id == productId);
            return product?.Stock ?? 0;
        }

        public Dictionary<string, int> GetStockMovementsSummary(int productId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Where(t => t.ProductId == productId);

            if (startDate.HasValue)
            {
                query = query.Where(t => t.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.CreatedDate <= endDate.Value);
            }

            var transactions = query.ToList();

            var summary = new Dictionary<string, int>
            {
                { "Import", 0 },
                { "Export", 0 },
                { "Sale", 0 },
                { "Adjustment_Increase", 0 },
                { "Adjustment_Decrease", 0 },
                { "Return", 0 }
            };

            foreach (var transaction in transactions)
            {
                if (summary.ContainsKey(transaction.TransactionType))
                {
                    summary[transaction.TransactionType] += transaction.Quantity;
                }
            }

            return summary;
        }
    }
}

