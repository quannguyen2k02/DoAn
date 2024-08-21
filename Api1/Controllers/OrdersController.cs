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

namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BanHangContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(BanHangContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


         [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            // Lấy UserId từ token đã xác thực
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            // Tạo order mới
            var order = new Order
            {
                CustomerId = userId,
                CustomerName = orderRequest.CustomerName,
                Phone = orderRequest.Phone,
                Address = orderRequest.Address,
                Email = orderRequest.Email,
                TotalAmount = orderRequest.Items.Sum(item => item.Quantity * (_context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Price ?? 0)),
                Quantity = orderRequest.Items.Sum(item => item.Quantity),
                TypePayment = orderRequest.TypePayment,
                Status = 1, // Ví dụ: Trạng thái mới
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                OrderDetails = orderRequest.Items.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = _context.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Price ?? 0,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                }).ToList()
            };

            // Lưu order vào cơ sở dữ liệu
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }


    }
}
