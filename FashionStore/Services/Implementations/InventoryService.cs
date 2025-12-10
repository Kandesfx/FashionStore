using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<InventoryTransaction> GetTransactions(int? productId = null, string transactionType = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<InventoryTransaction> transactions;

            if (productId.HasValue && startDate.HasValue && endDate.HasValue)
            {
                transactions = _unitOfWork.InventoryTransactions.GetByProductAndDateRange(productId.Value, startDate.Value, endDate.Value);
            }
            else if (productId.HasValue)
            {
                transactions = _unitOfWork.InventoryTransactions.GetByProductId(productId.Value);
            }
            else if (startDate.HasValue && endDate.HasValue)
            {
                transactions = _unitOfWork.InventoryTransactions.GetByDateRange(startDate.Value, endDate.Value);
            }
            else if (!string.IsNullOrEmpty(transactionType))
            {
                transactions = _unitOfWork.InventoryTransactions.GetByTransactionType(transactionType);
            }
            else
            {
                transactions = _unitOfWork.InventoryTransactions.GetAll();
            }

            return transactions.OrderByDescending(t => t.CreatedDate);
        }

        public InventoryTransaction GetTransactionById(int id)
        {
            return _unitOfWork.InventoryTransactions.GetById(id);
        }

        public InventoryTransaction ImportStock(int productId, int quantity, string notes, int? userId = null, int? productVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng nhập kho phải lớn hơn 0.");

            var product = _unitOfWork.Products.GetById(productId);
            if (product == null)
                throw new ArgumentException("Sản phẩm không tồn tại.");

            ProductVariant variant = null;
            if (productVariantId.HasValue)
            {
                variant = _unitOfWork.ProductVariants.GetById(productVariantId.Value);
                if (variant == null || variant.ProductId != productId)
                    throw new ArgumentException("Biến thể sản phẩm không hợp lệ.");
            }

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                TransactionType = "Import",
                Quantity = quantity,
                Notes = notes,
                CreatedByUserId = userId,
                CreatedDate = DateTime.Now
            };

            _unitOfWork.InventoryTransactions.Add(transaction);

            // Update stock (variant nếu có, đồng thời cập nhật tổng Product.Stock để đồng bộ)
            if (variant != null)
            {
                variant.Stock += quantity;
                _unitOfWork.ProductVariants.Update(variant);
            }

            product.Stock += quantity;
            _unitOfWork.Products.Update(product);

            _unitOfWork.Complete();

            return transaction;
        }

        public InventoryTransaction ExportStock(int productId, int quantity, string notes, int? userId = null, int? productVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng xuất kho phải lớn hơn 0.");

            if (!IsStockAvailable(productId, quantity, productVariantId))
                throw new InvalidOperationException("Không đủ tồn kho để xuất.");

            var product = _unitOfWork.Products.GetById(productId);
            if (product == null)
                throw new ArgumentException("Sản phẩm không tồn tại.");

            ProductVariant variant = null;
            if (productVariantId.HasValue)
            {
                variant = _unitOfWork.ProductVariants.GetById(productVariantId.Value);
                if (variant == null || variant.ProductId != productId)
                    throw new ArgumentException("Biến thể sản phẩm không hợp lệ.");
            }

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                TransactionType = "Export",
                Quantity = quantity,
                Notes = notes,
                CreatedByUserId = userId,
                CreatedDate = DateTime.Now
            };

            _unitOfWork.InventoryTransactions.Add(transaction);

            // Update stock
            if (variant != null)
            {
                variant.Stock -= quantity;
                if (variant.Stock < 0) variant.Stock = 0;
                _unitOfWork.ProductVariants.Update(variant);
            }

            product.Stock -= quantity;
            if (product.Stock < 0) product.Stock = 0;
            _unitOfWork.Products.Update(product);

            _unitOfWork.Complete();

            return transaction;
        }

        public InventoryTransaction AdjustStock(int productId, int quantity, string reason, int? userId = null, int? productVariantId = null)
        {
            if (quantity == 0)
                throw new ArgumentException("Số lượng điều chỉnh không được bằng 0.");

            var product = _unitOfWork.Products.GetById(productId);
            if (product == null)
                throw new ArgumentException("Sản phẩm không tồn tại.");

            ProductVariant variant = null;
            if (productVariantId.HasValue)
            {
                variant = _unitOfWork.ProductVariants.GetById(productVariantId.Value);
                if (variant == null || variant.ProductId != productId)
                    throw new ArgumentException("Biến thể sản phẩm không hợp lệ.");
            }

            string transactionType = quantity > 0 ? "Adjustment_Increase" : "Adjustment_Decrease";
            int absQuantity = Math.Abs(quantity);

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                TransactionType = transactionType,
                Quantity = absQuantity,
                Notes = reason,
                CreatedByUserId = userId,
                CreatedDate = DateTime.Now
            };

            _unitOfWork.InventoryTransactions.Add(transaction);

            // Update stock
            if (variant != null)
            {
                variant.Stock += quantity;
                if (variant.Stock < 0) variant.Stock = 0;
                _unitOfWork.ProductVariants.Update(variant);
            }

            product.Stock += quantity;
            if (product.Stock < 0) product.Stock = 0;
            _unitOfWork.Products.Update(product);

            _unitOfWork.Complete();

            return transaction;
        }

        public InventoryTransaction RecordSale(int productId, int quantity, int? orderId = null, string notes = null, int? productVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng bán phải lớn hơn 0.");

            if (!IsStockAvailable(productId, quantity, productVariantId))
                throw new InvalidOperationException("Không đủ tồn kho để bán.");

            var product = _unitOfWork.Products.GetById(productId);
            if (product == null)
                throw new ArgumentException("Sản phẩm không tồn tại.");

            ProductVariant variant = null;
            if (productVariantId.HasValue)
            {
                variant = _unitOfWork.ProductVariants.GetById(productVariantId.Value);
                if (variant == null || variant.ProductId != productId)
                    throw new ArgumentException("Biến thể sản phẩm không hợp lệ.");
            }

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                TransactionType = "Sale",
                Quantity = quantity,
                OrderId = orderId,
                Notes = notes ?? $"Bán hàng - Đơn hàng #{orderId}",
                CreatedDate = DateTime.Now
            };

            _unitOfWork.InventoryTransactions.Add(transaction);

            // Update stock
            if (variant != null)
            {
                variant.Stock -= quantity;
                if (variant.Stock < 0) variant.Stock = 0;
                _unitOfWork.ProductVariants.Update(variant);
            }

            product.Stock -= quantity;
            if (product.Stock < 0) product.Stock = 0;
            _unitOfWork.Products.Update(product);

            _unitOfWork.Complete();

            return transaction;
        }

        public InventoryTransaction RecordReturn(int productId, int quantity, int? orderId = null, string notes = null, int? productVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng trả hàng phải lớn hơn 0.");

            var product = _unitOfWork.Products.GetById(productId);
            if (product == null)
                throw new ArgumentException("Sản phẩm không tồn tại.");

            ProductVariant variant = null;
            if (productVariantId.HasValue)
            {
                variant = _unitOfWork.ProductVariants.GetById(productVariantId.Value);
                if (variant == null || variant.ProductId != productId)
                    throw new ArgumentException("Biến thể sản phẩm không hợp lệ.");
            }

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                TransactionType = "Return",
                Quantity = quantity,
                OrderId = orderId,
                Notes = notes ?? $"Trả hàng - Đơn hàng #{orderId}",
                CreatedDate = DateTime.Now
            };

            _unitOfWork.InventoryTransactions.Add(transaction);

            if (variant != null)
            {
                variant.Stock += quantity;
                _unitOfWork.ProductVariants.Update(variant);
            }

            product.Stock += quantity;
            _unitOfWork.Products.Update(product);

            _unitOfWork.Complete();

            return transaction;
        }

        public int GetCurrentStock(int productId, int? productVariantId = null)
        {
            return _unitOfWork.InventoryTransactions.GetCurrentStock(productId, productVariantId);
        }

        public IEnumerable<Product> GetLowStockProducts(int? threshold = null)
        {
            int stockThreshold = threshold ?? 10; // Default threshold
            return _unitOfWork.InventoryTransactions.GetLowStockProducts(stockThreshold);
        }

        public Dictionary<string, int> GetStockMovementsSummary(int productId, DateTime? startDate = null, DateTime? endDate = null, int? productVariantId = null)
        {
            var query = _unitOfWork.InventoryTransactions.GetAll()
                .Where(t => t.ProductId == productId);

            if (productVariantId.HasValue)
            {
                query = query.Where(t => t.ProductVariantId == productVariantId.Value);
            }

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

        public bool IsStockAvailable(int productId, int quantity, int? productVariantId = null)
        {
            int currentStock = GetCurrentStock(productId, productVariantId);
            return currentStock >= quantity;
        }

        public bool ValidateStockOperation(int productId, string transactionType, int quantity, int? productVariantId = null)
        {
            if (quantity <= 0)
                return false;

            // For export/sale operations, check stock availability
            if (transactionType == "Export" || transactionType == "Sale")
            {
                return IsStockAvailable(productId, quantity, productVariantId);
            }

            return true;
        }
    }
}

