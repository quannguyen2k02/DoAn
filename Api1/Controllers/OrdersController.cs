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
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDto)
        {
            var userId = _userManager.GetUserId(User);
            // Tạo order mới từ DTO
            var order = new Order
            {
                CustomerId = userId,
                CustomerName = orderDto.CustomerName,
                Phone = orderDto.Phone,
                Address = orderDto.Address,
                CreatedDate = DateTime.UtcNow,
                Email = orderDto.Email,
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
    }
}





