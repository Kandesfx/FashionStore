using FashionStore.Models.Entities;
using System;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IInventoryTransactionRepository : IRepository<InventoryTransaction>
    {
        // Get transactions by product
        IEnumerable<InventoryTransaction> GetByProductId(int productId);
        
        // Get transactions by type
        IEnumerable<InventoryTransaction> GetByTransactionType(string transactionType);
        
        // Get transactions by date range
        IEnumerable<InventoryTransaction> GetByDateRange(DateTime startDate, DateTime endDate);
        
        // Get transactions by product and date range
        IEnumerable<InventoryTransaction> GetByProductAndDateRange(int productId, DateTime startDate, DateTime endDate);
        
        // Get low stock products (below threshold)
        IEnumerable<Product> GetLowStockProducts(int threshold);
        
        // Get current stock for product or variant
        int GetCurrentStock(int productId, int? productVariantId = null);
        
        // Get stock movements summary
        Dictionary<string, int> GetStockMovementsSummary(int productId, DateTime? startDate = null, DateTime? endDate = null);
    }
}

