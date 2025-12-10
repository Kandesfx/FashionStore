using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantService _productVariantService;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, 
            IProductRepository productRepository, IProductVariantService productVariantService, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _productVariantService = productVariantService;
            _unitOfWork = unitOfWork;
        }

        public Cart GetCartByUserId(int userId)
        {
            return _cartRepository.GetByUserId(userId);
        }

        public Cart CreateCart(int userId)
        {
            var cart = new Cart
            {
                UserId = userId,
                CreatedDate = DateTime.Now
            };
            _cartRepository.Add(cart);
            _unitOfWork.Complete();
            return cart;
        }

        public void AddToCart(int userId, int productId, int quantity, int? productVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");

            var product = _productRepository.GetById(productId);
            if (product == null || !product.IsActive)
                throw new InvalidOperationException("Sản phẩm không tồn tại hoặc đã ngừng bán");

            int availableStock = product.Stock;
            decimal itemPrice = product.Price;

            // If variant is selected, use variant stock and price
            if (productVariantId.HasValue)
            {
                var variants = _productVariantService.GetByProductId(productId);
                var variant = variants.FirstOrDefault(v => v.Id == productVariantId.Value && v.IsActive);
                if (variant == null)
                    throw new InvalidOperationException("Biến thể sản phẩm không tồn tại");

                availableStock = variant.Stock;
                if (variant.Price.HasValue)
                {
                    itemPrice = variant.Price.Value;
                }
            }

            if (availableStock < quantity)
                throw new InvalidOperationException("Số lượng tồn kho không đủ");

            var cart = GetCartByUserId(userId);
            if (cart == null)
            {
                cart = CreateCart(userId);
            }

            // Check if product (with same variant) already in cart
            var existingItem = _cartItemRepository.SingleOrDefault(ci => 
                ci.CartId == cart.Id && 
                ci.ProductId == productId && 
                ci.ProductVariantId == productVariantId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (existingItem.Quantity > availableStock)
                    throw new InvalidOperationException("Số lượng vượt quá tồn kho");
                _cartItemRepository.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    ProductVariantId = productVariantId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                };
                _cartItemRepository.Add(cartItem);
            }

            cart.UpdatedDate = DateTime.Now;
            _cartRepository.Update(cart);
            _unitOfWork.Complete();
        }

        public void UpdateCartItem(int cartItemId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");

            var cartItem = _cartItemRepository.GetById(cartItemId);
            if (cartItem == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại trong giỏ hàng");

            var product = _productRepository.GetById(cartItem.ProductId);
            if (product.Stock < quantity)
                throw new InvalidOperationException("Số lượng tồn kho không đủ");

            cartItem.Quantity = quantity;
            _cartItemRepository.Update(cartItem);

            var cart = _cartRepository.GetById(cartItem.CartId);
            cart.UpdatedDate = DateTime.Now;
            _cartRepository.Update(cart);
            _unitOfWork.Complete();
        }

        public void RemoveCartItem(int cartItemId)
        {
            var cartItem = _cartItemRepository.GetById(cartItemId);
            if (cartItem == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại trong giỏ hàng");

            var cartId = cartItem.CartId;
            _cartItemRepository.Remove(cartItem);

            var cart = _cartRepository.GetById(cartId);
            cart.UpdatedDate = DateTime.Now;
            _cartRepository.Update(cart);
            _unitOfWork.Complete();
        }

        public void ClearCart(int userId)
        {
            var cart = GetCartByUserId(userId);
            if (cart != null)
            {
                var items = _cartItemRepository.Find(ci => ci.CartId == cart.Id).ToList();
                foreach (var item in items)
                {
                    _cartItemRepository.Remove(item);
                }
                cart.UpdatedDate = DateTime.Now;
                _cartRepository.Update(cart);
                _unitOfWork.Complete();
            }
        }

        public CartViewModel GetCartViewModel(int userId)
        {
            var cart = GetCartByUserId(userId);
            if (cart == null)
            {
                return new CartViewModel
                {
                    CartId = 0,
                    Items = new List<CartItemViewModel>(),
                    TotalAmount = 0,
                    TotalItems = 0
                };
            }

            var items = _cartItemRepository.GetCartItemsWithProduct(cart.Id)
                .Select(ci => new CartItemViewModel
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.ProductName,
                    ImageUrl = ci.Product.ImageUrl,
                    Price = ci.Product.Price,
                    DiscountPrice = ci.Product.DiscountPrice,
                    Quantity = ci.Quantity,
                    Stock = ci.ProductVariantId.HasValue && ci.ProductVariant != null 
                        ? ci.ProductVariant.Stock 
                        : ci.Product.Stock,
                    ProductVariantId = ci.ProductVariantId,
                    Size = ci.ProductVariant != null ? ci.ProductVariant.Size : null,
                    Color = ci.ProductVariant != null ? ci.ProductVariant.Color : null,
                    SubTotal = ci.Quantity * (ci.ProductVariantId.HasValue && ci.ProductVariant != null && ci.ProductVariant.Price.HasValue
                        ? ci.ProductVariant.Price.Value
                        : ci.Product.FinalPrice)
                }).ToList();

            return new CartViewModel
            {
                CartId = cart.Id,
                Items = items,
                TotalAmount = items.Sum(i => i.SubTotal),
                TotalItems = items.Sum(i => i.Quantity)
            };
        }

        public decimal CalculateCartTotal(int userId)
        {
            var cart = GetCartByUserId(userId);
            if (cart == null)
                return 0;

            var items = _cartItemRepository.GetCartItemsWithProduct(cart.Id).ToList();
            return items.Sum(ci => ci.Quantity * ci.Product.FinalPrice);
        }

        public int GetCartItemCount(int userId)
        {
            var cart = GetCartByUserId(userId);
            if (cart == null)
                return 0;

            // Return total quantity of all items, not count of distinct items
            var items = _cartItemRepository.Find(ci => ci.CartId == cart.Id).ToList();
            return items.Sum(ci => ci.Quantity);
        }

        public bool ValidateCart(int userId)
        {
            var cart = GetCartByUserId(userId);
            if (cart == null)
                return false;

            var items = _cartItemRepository.GetCartItemsWithProduct(cart.Id).ToList();
            foreach (var item in items)
            {
                if (item.Product.Stock < item.Quantity)
                    return false;
            }
            return true;
        }
    }
}

