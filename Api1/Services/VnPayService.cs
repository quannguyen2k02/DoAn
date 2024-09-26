using API.Models;
using Api1.Data;
using Api1.Models.VnPay;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api1.Services
{
    public class VnPayService:IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly BanHangContext _context;

        public VnPayService(IConfiguration configuration, BanHangContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public string CreatePaymentUrl(OrderDTO model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
            // Tạo order mới từ DTO
            var order = new Order
            {
                OrderCode = DateTime.UtcNow.Ticks.ToString(),
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Address = model.Address,
                CreatedDate = DateTime.Now,
                Email = model.Email,
                TypePayment = 0,
                TotalAmount = model.Items.Sum(item => item.Quantity * (_context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0)),
                OrderDetails = model.Items.Select(item => new OrderDetail
                {
                    CreatedDate = DateTime.Now,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = _context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0
                }).ToList()
            };

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)order.TotalAmount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.CustomerName} ");
            pay.AddRequestData("vnp_OrderType", model.Phone);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }
        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }
    }
}
