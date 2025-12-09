using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FashionStore.Models.Momo;
using FashionStore.Services.Interfaces;
using Newtonsoft.Json;

namespace FashionStore.Services.Implementations
{
    public class MomoPaymentService : IMomoPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _partnerCode;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _endpoint;
        private readonly string _redirectUrl;
        private readonly string _ipnUrl;

        public MomoPaymentService(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _partnerCode = ConfigurationManager.AppSettings["Momo:PartnerCode"];
            _accessKey = ConfigurationManager.AppSettings["Momo:AccessKey"];
            _secretKey = ConfigurationManager.AppSettings["Momo:SecretKey"];
            _endpoint = ConfigurationManager.AppSettings["Momo:Endpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/create";
            _redirectUrl = ConfigurationManager.AppSettings["Momo:RedirectUrl"];
            _ipnUrl = ConfigurationManager.AppSettings["Momo:IpnUrl"];
        }

        public async Task<MomoCreatePaymentResponse> CreatePaymentAsync(string orderId, decimal amount, string orderInfo)
        {
            if (string.IsNullOrEmpty(_partnerCode) || string.IsNullOrEmpty(_accessKey) || string.IsNullOrEmpty(_secretKey))
            {
                throw new InvalidOperationException("Chưa cấu hình thông tin MoMo (partnerCode, accessKey, secretKey).");
            }

            var requestId = Guid.NewGuid().ToString("N");
            var request = new MomoCreatePaymentRequest
            {
                PartnerCode = _partnerCode,
                AccessKey = _accessKey,
                RequestId = requestId,
                Amount = ((long)amount).ToString(),
                OrderId = orderId,
                OrderInfo = orderInfo,
                RedirectUrl = _redirectUrl,
                IpnUrl = _ipnUrl,
                ExtraData = string.Empty,
                RequestType = "captureWallet"
            };

            var rawSignature =
                $"accessKey={request.AccessKey}&amount={request.Amount}&extraData={request.ExtraData}&ipnUrl={request.IpnUrl}&orderId={request.OrderId}&orderInfo={request.OrderInfo}&partnerCode={request.PartnerCode}&redirectUrl={request.RedirectUrl}&requestId={request.RequestId}&requestType={request.RequestType}";

            request.Signature = SignHmacSha256(rawSignature, _secretKey);

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(_endpoint, content))
            {
                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"MoMo trả lỗi HTTP {(int)response.StatusCode}: {body}");
                }

                return JsonConvert.DeserializeObject<MomoCreatePaymentResponse>(body);
            }
        }

        public bool ValidateIpn(MomoIpnRequest ipnRequest)
        {
            if (ipnRequest == null)
                return false;

            // rawSignature thứ tự do MoMo yêu cầu cho IPN
            var raw =
                $"accessKey={_accessKey}&amount={ipnRequest.Amount}&extraData={ipnRequest.ExtraData}&message={ipnRequest.Message}&orderId={ipnRequest.OrderId}&orderInfo={ipnRequest.OrderInfo}&orderType={ipnRequest.OrderType}&partnerCode={ipnRequest.PartnerCode}&payType={ipnRequest.PayType}&requestId={ipnRequest.RequestId}&responseTime={ipnRequest.ResponseTime}&resultCode={ipnRequest.ResultCode}&transId={ipnRequest.TransId}";

            var expectedSignature = SignHmacSha256(raw, _secretKey);
            return string.Equals(expectedSignature, ipnRequest.Signature, StringComparison.OrdinalIgnoreCase);
        }

        private static string SignHmacSha256(string raw, string secret)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(raw))).Replace("-", "").ToLower();
            }
        }
    }
}

