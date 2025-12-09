using System.Threading.Tasks;
using FashionStore.Models.Momo;

namespace FashionStore.Services.Interfaces
{
    public interface IMomoPaymentService
    {
        Task<MomoCreatePaymentResponse> CreatePaymentAsync(string orderId, decimal amount, string orderInfo);
        bool ValidateIpn(MomoIpnRequest ipnRequest);
    }
}

