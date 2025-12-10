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
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, 
            IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
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

        public void AddToCart(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");

            var product = _productRepository.GetById(productId);
            if (product == null || !product.IsActive)
                throw new InvalidOperationException("Sản phẩm không tồn tại hoặc đã ngừng bán");

            if (product.Stock < quantity)
                throw new InvalidOperationException("Số lượng tồn kho không đủ");

            var cart = GetCartByUserId(userId);
            if (cart == null)
            {
                cart = CreateCart(userId);
            }

            // Check if product already in cart
            var existingItem = _cartItemRepository.SingleOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (existingItem.Quantity > product.Stock)
                    throw new InvalidOperationException("Số lượng vượt quá tồn kho");
                _cartItemRepository.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
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
                    Stock = ci.Product.Stock,
                    SubTotal = ci.Quantity * ci.Product.FinalPrice
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

