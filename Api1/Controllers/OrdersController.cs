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
using System.Net;
using Api1.Services;
using Api1.Models.VnPay;

namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BanHangContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<MomoOptionModel> _options;
        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _configuration;

        public OrdersController(BanHangContext context, UserManager<ApplicationUser> userManager, IOptions<MomoOptionModel> options, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _options = options;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize(Roles = AppRole.Customer + "," + AppRole.Admin)]
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
                TypePayment = 2,
                TotalAmount = orderDto.Items.Sum(item => item.Quantity * (_context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.PriceSale ?? 0)),
                OrderDetails = orderDto.Items.Select(item => new OrderDetail
                {
                CreatedDate = DateTime.Now,
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
                    CreatedDate = DateTime.Now,
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

        [Authorize(Roles = AppRole.Customer + "," + AppRole.Admin)]
        [HttpPost("vnpay")]
        public async Task<IActionResult> CreatePaymentvnpayUrl(OrderDTO model)
        {
            var userId = _userManager.GetUserId(User);
            // Tạo order mới từ DTO
            var order = new Order
            {
                OrderCode = DateTime.UtcNow.Ticks.ToString(),
                CustomerId = userId,
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
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            var context = this.HttpContext;
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
            double total = Convert.ToDouble(order.TotalAmount) * 100;
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", total.ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{order.CustomerName} {order.Address} {order.TotalAmount}");
            pay.AddRequestData("vnp_OrderType", "dq");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", order.OrderCode);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return Ok(paymentUrl);
        }

        [HttpGet("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            IQueryCollection collections = Request.Query;
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderCode == response.OrderId);
            if(response.Success)
            {
                order.TypePayment = 1;
            }
            else
            {
                _context.Remove(order);

            }
            _context.SaveChangesAsync();

            var queryParams = new Dictionary<string, string>
    {
        { "orderId", response.OrderId },
        { "success", response.Success.ToString() },
        { "transactionId", response.TransactionId }
    };

            // Tạo URL với query string
            var queryString = string.Join("&", queryParams.Select(kv => $"{kv.Key}={kv.Value}"));

            return Redirect($"http://localhost:5173/resultcheckout?{queryString}");

        }

        //[HttpGet("PaymentCallBack")]
        //public async Task<IActionResult> PaymentCallBack([FromQuery] MomoExecuteResponseModel model)
        //{

        //    // Kiểm tra mã lỗi
        //    if (model.ErrorCode == "0")
        //    {
        //        // Xử lý đơn hàng thành công
        //        // Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu
        //        // ...
        //        // Tạo order mới từ DTO
        //        var order = _context.Orders.FirstOrDefault(x => x.OrderCode == model.OrderId);
        //        if (order == null)
        //        {
        //            return BadRequest("Có lỗi xảy ra.");
        //        }
        //        order.TypePayment = 1;
        //        _context.SaveChanges();
        //        return Redirect($"http://localhost:5173/user");
        //    }
        //    else
        //    {
        //        var order = _context.Orders.FirstOrDefault(x => x.OrderCode == model.OrderId);
        //        if (order == null)
        //        {
        //            return BadRequest("Có lỗi xảy ra.");
        //        }
        //        _context.Orders.Remove(order);
        //        _context.SaveChanges();
        //        return Redirect("http://localhost:5173");
        //    }
        //}
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
            if (listOrder == null)
            {
                return NotFound();
            }
            return await listOrder.ToListAsync();
        }

        [HttpGet("GetAllOrders")]

        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrder()
        {

            if (_context.Orders == null)
            {
                return NotFound();
            }

            return await _context.Orders.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ComfirmOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            order.Status = 1;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái thành công!" });
        }
        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> TransferOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            order.Status = 2;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái thành công!" });
        }
        [HttpPost("{id}/done")]
        public async Task<IActionResult> DoneOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            order.Status = 3;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái thành công!" });
        }
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            order.Status = 4;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái thành công!" });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }
            return order;
        }
        [HttpPut("{orderId}")]
        [Authorize(Roles = AppRole.Admin)]

        public async Task<IActionResult> PutOrder([FromBody] OrderDTO orderDto, int orderId, int status)
        {
            var order = _context.Orders.FirstOrDefault(x=>x.Id == orderId);
            if(order == null)
            {
                return NotFound();
            }
            order.CustomerName = orderDto.CustomerName;
            order.Phone = orderDto.Phone;
            order.Address = orderDto.Address;
            order.ModifiedDate = DateTime.Now;
            order.Email = orderDto.Email;
            order.Status = status;
            await _context.SaveChangesAsync();
            return Ok(order);
        }
        [HttpGet("GetUnconfirmOrder")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult<IEnumerable<Order>>> GetUnconfirmedOrder()
        {

            if (_context.Orders == null)
            {
                return NotFound();
            }
            var listOrder = _context.Orders.Where(x => x.Status == 0).OrderByDescending(x => x.CreatedDate);
            if (listOrder == null)
            {
                return NotFound();
            }
            return await listOrder.ToListAsync();
        }
        [HttpGet("GetConfirmedOrder")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult<IEnumerable<Order>>> GetConfirmedOrder()
        {

            if (_context.Orders == null)
            {
                return NotFound();
            }
            var listOrder = _context.Orders.Where(x => x.Status == 1).OrderByDescending(x => x.CreatedDate);
            if (listOrder == null)
            {
                return NotFound();
            }
            return await listOrder.ToListAsync();
        }
        [HttpGet("GetShippingOrder")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult<IEnumerable<Order>>> GetShippingOrder()
        {

            if (_context.Orders == null)
            {
                return NotFound();
            }
            var listOrder = _context.Orders.Where(x => x.Status == 2).OrderByDescending(x => x.CreatedDate);
            if (listOrder == null)
            {
                return NotFound();
            }
            return await listOrder.ToListAsync();
        }

        [HttpGet("GetDeliveredOrder")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult<IEnumerable<Order>>> GetDeliveredOrder()
        {

            if (_context.Orders == null)
            {
                return NotFound();
            }
            var listOrder = _context.Orders.Where(x => x.Status == 3).OrderByDescending(x => x.CreatedDate);
            if (listOrder == null)
            {
                return NotFound();
            }
            return await listOrder.ToListAsync();
        }
        [HttpDelete("{OrderId}")]
        public async Task<IActionResult> DeleteOrder(int OrderId)
        {
            // Kiểm tra xem bảng Orders có null không
            if (_context.Orders == null)
            {
                return NotFound("Orders table not found.");
            }

            // Tìm order có Id là OrderId
            var order = await _context.Orders.FindAsync(OrderId);

            // Kiểm tra nếu không tìm thấy đơn hàng
            if (order == null)
            {
                return NotFound($"Order with ID = {OrderId} not found.");
            }

            // Xóa order
            _context.Orders.Remove(order);

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            // Trả về trạng thái NoContent (204) sau khi xóa thành công
            return NoContent();
        }

        [HttpGet("GetOrderByCustomerId/{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrderByCustomer(string id)
        {

            if (_context.Orders == null)
            {
                return NotFound();
            }
            var listOrder = _context.Orders.Where(x => x.CustomerId == id).OrderByDescending(x => x.CreatedDate);
            if (listOrder == null)
            {
                return NotFound();
            }
            return await listOrder.ToListAsync();
        }

    }
}
