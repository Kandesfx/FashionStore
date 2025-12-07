using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface ICartService
    {
        Cart GetCartByUserId(int userId);
        Cart CreateCart(int userId);
        void AddToCart(int userId, int productId, int quantity);
        void UpdateCartItem(int cartItemId, int quantity);
        void RemoveCartItem(int cartItemId);
        void ClearCart(int userId);
        
        CartViewModel GetCartViewModel(int userId);
        decimal CalculateCartTotal(int userId);
        int GetCartItemCount(int userId);
        bool ValidateCart(int userId);
    }
}

