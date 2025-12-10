using FashionStore.Models.Entities;
using System;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface IInventoryService
    {
        // Get transactions
        IEnumerable<InventoryTransaction> GetTransactions(int? productId = null, string transactionType = null, DateTime? startDate = null, DateTime? endDate = null);
        InventoryTransaction GetTransactionById(int id);
        
        // Stock operations
        InventoryTransaction ImportStock(int productId, int quantity, string notes, int? userId = null, int? productVariantId = null);
        InventoryTransaction ExportStock(int productId, int quantity, string notes, int? userId = null, int? productVariantId = null);
        InventoryTransaction AdjustStock(int productId, int quantity, string reason, int? userId = null, int? productVariantId = null);
        InventoryTransaction RecordSale(int productId, int quantity, int? orderId = null, string notes = null, int? productVariantId = null);
        InventoryTransaction RecordReturn(int productId, int quantity, int? orderId = null, string notes = null, int? productVariantId = null);
        
        // Stock queries
        int GetCurrentStock(int productId, int? productVariantId = null);
        IEnumerable<Product> GetLowStockProducts(int? threshold = null);
        Dictionary<string, int> GetStockMovementsSummary(int productId, DateTime? startDate = null, DateTime? endDate = null, int? productVariantId = null);
        
        // Stock validation
        bool IsStockAvailable(int productId, int quantity, int? productVariantId = null);
        bool ValidateStockOperation(int productId, string transactionType, int quantity, int? productVariantId = null);
    }
}

