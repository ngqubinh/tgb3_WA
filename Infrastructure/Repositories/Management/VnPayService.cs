using Application.Interfaces.Management;
using Domain.Constants;
using Domain.Models.Enum;
using Domain.Models.Management;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories.Management
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext httpContext, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
                vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
                vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
                vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());                
                vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
                vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + model.OrderId);
                vnpay.AddRequestData("vnp_OrderType", "other"); 
                vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
                vnpay.AddRequestData("vnp_TxnRef", tick);
                vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

                string url = _config["VnPay:BaseUrl"];
                var paymentUrl = vnpay.CreateRequestUrl(url, _config["VnPay:HashSecret"]);
                return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach(var (key, value) in collections)
            {
                if(!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToUInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");


            Console.WriteLine($"vnp_orderId: {vnp_orderId}"); 
            Console.WriteLine($"vnp_TransactionId: {vnp_TransactionId}"); 
            Console.WriteLine($"vnp_SecureHash: {vnp_SecureHash}"); 
            Console.WriteLine($"vnp_ResponseCode: {vnp_ResponseCode}"); 
            Console.WriteLine($"vnp_OrderInfo: {vnp_OrderInfo}");
            Console.WriteLine($"vnp_TransactionStatus: {vnp_TransactionStatus}");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if(!checkSignature)
            {
                return new VnPaymentResponseModel()
                {
                    Success = false
                };
            }
            return new VnPaymentResponseModel() 
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
                VnStatus = vnp_TransactionStatus
            };
        }
    }
}