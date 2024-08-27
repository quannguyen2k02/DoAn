using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Api1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using Api1.Models.Momo;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Api1.Models;

namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BanHangContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<MomoOptionModel> _options;


        public OrdersController(BanHangContext context, UserManager<ApplicationUser> userManager, IOptions<MomoOptionModel> options)
        {
            _context = context;
            _userManager = userManager;
            _options = options;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDto)
        {
            var userId = _userManager.GetUserId(User);
            // Tạo order mới từ DTO
            var order = new Order
            {
                OrderCode = DateTime.UtcNow.Ticks.ToString(),
                CustomerId = userId,
                CustomerName = orderDto.CustomerName,
                Phone = orderDto.Phone,
                Address = orderDto.Address,
                CreatedDate = DateTime.Now,
                Email = orderDto.Email,
                TypePayment = 0,
                TotalAmount = orderDto.Items.Sum(item => item.Quantity * (_context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0)),
                OrderDetails = orderDto.Items.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = _context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0
                }).ToList()
            };

            // Lưu order vào cơ sở dữ liệu
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }
        //[Authorize(Roles =AppRole.Customer)]
        //[HttpPost("checkout")]
        //public async Task<IActionResult> CreatePaymentUrl(OrderDTO orderDto)
        //{
        //    var userId = _userManager.GetUserId(User);
        //    // Tạo order mới từ DTO
        //    var order = new Order
        //    {
        //        CustomerId = userId,
        //        CustomerName = orderDto.CustomerName,
        //        Phone = orderDto.Phone,
        //        Address = orderDto.Address,
        //        CreatedDate = DateTime.UtcNow,
        //        Email = orderDto.Email,
        //        TotalAmount = orderDto.Items.Sum(item => item.Quantity * (_context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0)),
        //        OrderDetails = orderDto.Items.Select(item => new OrderDetail
        //        {
        //            ProductId = item.ProductId,
        //            Quantity = item.Quantity,
        //            Price = _context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0
        //        }).ToList()
        //    };

        //    string infor = "Khách hàng: " + order.CustomerName + ". Nội dung: thanh toán ";
        //    string orderid = DateTime.UtcNow.Ticks.ToString();
        //    var rawData =
        //        $"partnerCode={_options.Value.PartnerCode}&accessKey={_options.Value.AccessKey}&requestId={orderid}&amount={order.TotalAmount}&orderId={orderid}&orderInfo={infor}&returnUrl={_options.Value.ReturnUrl}&notifyUrl={_options.Value.NotifyUrl}&extraData=";

        //    var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

        //    var client = new RestClient(_options.Value.MomoApiUrl);
        //    var request = new RestRequest() { Method = RestSharp.Method.Post };
        //    request.AddHeader("Content-Type", "application/json; charset=UTF-8");
        //    // Create an object representing the request data
        //    var requestData = new
        //    {
        //        accessKey = _options.Value.AccessKey,
        //        partnerCode = _options.Value.PartnerCode,
        //        requestType = _options.Value.RequestType,
        //        notifyUrl = _options.Value.NotifyUrl,
        //        returnUrl = _options.Value.ReturnUrl,
        //        orderId = orderid,
        //        amount =Convert.ToDouble(order.TotalAmount).ToString(),
        //        orderInfo = infor,
        //        requestId = orderid,
        //        extraData = "",
        //        signature = signature
        //    };

        //    request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

        //    var response = await client.ExecuteAsync(request);
        //    var result = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
        //    return Ok(result.PayUrl);
        //}

        [Authorize(Roles = AppRole.Customer)]
        [HttpPost("checkout")]
        public async Task<IActionResult> CreatePaymentUrl(OrderDTO orderDto)
        {
            var userId = _userManager.GetUserId(User);
            // Tạo order mới từ DTO
            var order = new Order
            {
                OrderCode = DateTime.UtcNow.Ticks.ToString(),
                CustomerId = userId,
                CustomerName = orderDto.CustomerName,
                Phone = orderDto.Phone,
                Address = orderDto.Address,
                CreatedDate = DateTime.Now,
                Email = orderDto.Email,
                TypePayment = 0,
                TotalAmount = orderDto.Items.Sum(item => item.Quantity * (_context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0)),
                OrderDetails = orderDto.Items.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = _context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0
                }).ToList()
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            string infor = "Khách hàng: " + order.CustomerName + ". Nội dung: thanh toán ";
            string orderid = order.OrderCode;
            var rawData =
                $"partnerCode={_options.Value.PartnerCode}&accessKey={_options.Value.AccessKey}&requestId={order.OrderCode}&amount={Convert.ToDouble(order.TotalAmount)}&orderId={orderid}&orderInfo={infor}&returnUrl={_options.Value.ReturnUrl}&notifyUrl={_options.Value.NotifyUrl}&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = RestSharp.Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            // Create an object representing the request data
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = order.OrderCode,
                amount = Convert.ToDouble(order.TotalAmount).ToString(),
                orderInfo = infor,
                requestId = order.OrderCode,
                extraData = "",
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            var result = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
            return Ok(result.PayUrl);
        }

        [HttpGet("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack([FromQuery] MomoExecuteResponseModel model)
        {

            // Kiểm tra mã lỗi
            if (model.ErrorCode == "0")
            {
                // Xử lý đơn hàng thành công
                // Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu
                // ...
                // Tạo order mới từ DTO
                var order = _context.Orders.FirstOrDefault(x=>x.OrderCode  == model.OrderId);
                if(order == null)
                {
                    return BadRequest("Có lỗi xảy ra.");
                }
                order.TypePayment = 1;
                _context.SaveChanges();
                return Redirect($"http://localhost:5173/checkoutresutl?id={model.OrderId}&amount={model.Amount}");
            }
            else
            {
                var order = _context.Orders.FirstOrDefault(x => x.OrderCode == model.OrderId);
                if (order == null)
                {
                    return BadRequest("Có lỗi xảy ra.");
                }
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return Redirect("http://localhost:5173");
            }
        }
        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }


        [HttpGet]
        [Authorize(Roles = AppRole.Customer)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            string userId = _userManager.GetUserId(User);

            if (_context.Orders == null)
            {
                return NotFound();
            }
            var listOrder = _context.Orders.Where(x => x.CustomerId == userId).OrderByDescending(x => x.CreatedDate);
            if(listOrder == null)
            {
                return NotFound();
            }
            return await listOrder.ToListAsync();
        }

       
    }
}





