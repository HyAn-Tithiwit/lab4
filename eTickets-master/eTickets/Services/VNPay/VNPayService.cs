using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using eTickets.Libraries;
using eTickets.Models.VNPay;

namespace eTickets.Services.VNPay
{
    public class VNPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VNPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var pay = new VNPayLibrary();
            var timeNow = DateTime.UtcNow.AddHours(7); // VN timezone
            var tick = DateTime.Now.Ticks.ToString();

            // Thêm thông tin đơn hàng
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} - {model.OrderDescription}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VNPayLibrary();
            return pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
        }
    }
}
