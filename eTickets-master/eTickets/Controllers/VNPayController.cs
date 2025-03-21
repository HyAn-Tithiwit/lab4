using Microsoft.AspNetCore.Mvc;
using eTickets.Models.VNPay;
using eTickets.Services.VNPay;

namespace eTickets.Controllers
{
    public class VNPayController : Controller
    {
        private readonly IVnPayService _vnPayService;

        public VNPayController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            return Json(new
            {
                success = response.Success,
                message = response.Success ? "Thanh toán thành công!" : "Thanh toán thất bại!",
                transactionId = response.TransactionId,
                orderId = response.OrderId,
                paymentMethod = response.PaymentMethod,
                paymentId = response.PaymentId,
                vnPayResponseCode = response.VnPayResponseCode
            });
        }
    }
}
