using API.Models;
using Api1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;

namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : Controller
    {
        private readonly BanHangContext _context;
        public DashboardController(BanHangContext context)
        {
            _context = context;
        }
        //Lấy ra sản phẩm bán chạy nhất
        [HttpGet("TrendingProduct")]
        public async Task<ActionResult<Product>> TrendingProduct()
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }

            // Lấy ProductID của sản phẩm bán chạy nhất
            var bestSellingProduct = _context.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductID = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .FirstOrDefault();

            // Nếu không có sản phẩm nào, trả về NotFound
            if (bestSellingProduct == null)
            {
                return NotFound();
            }

            // Truy vấn sản phẩm dựa vào ProductID của sản phẩm bán chạy nhất
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == bestSellingProduct.ProductID);

            // Nếu không tìm thấy sản phẩm, trả về NotFound
            if (product == null)
            {
                return NotFound();
            }
            var url = "https://localhost:7061/static/Contents/Images/Products/";
            product.Image = url + product.Image;
            // Trả về sản phẩm
            return Ok(new
            {
                Product = product,
                TotalQuantitySold = bestSellingProduct.TotalQuantity
            });
        }
        [HttpGet("TotalSalesRevenueByMonth")]
        public async Task<ActionResult<decimal>> TotalSalesRevenueByMonth()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            //var totalRevenue = _context.Orders.Sum(o => o.TotalAmount);

            //return Ok(totalRevenue);
            var totalRevenueByMonth = _context.Orders
    .GroupBy(o => new { o.CreatedDate.Year, o.CreatedDate.Month })
    .Select(g => new
    {
        Year = g.Key.Year,
        Month = g.Key.Month,
        TotalRevenue = g.Sum(o => o.TotalAmount)
    })
    .ToList();
            return Ok(totalRevenueByMonth);
        }

        [HttpGet("TotalSalesRevenueByDay")]
        public async Task<ActionResult<IEnumerable<object>>> TotalSalesRevenueByDay()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            // Truy vấn lấy tổng doanh thu theo ngày, sắp xếp theo CreatedDate tăng dần và lấy 5 ngày cuối cùng
            var totalRevenueByDay = _context.Orders
                .GroupBy(o => o.CreatedDate.Date) // Nhóm theo ngày
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Day = g.Key.Day,
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(g => g.Year) // Sắp xếp theo năm tăng dần
                .ThenBy(g => g.Month) // Sắp xếp theo tháng tăng dần
                .ThenBy(g => g.Day)   // Sắp xếp theo ngày tăng dần
                .Skip(Math.Max(0, _context.Orders
                    .GroupBy(o => o.CreatedDate.Date).Count() - 5)) // Bỏ qua các ngày trước đó để chỉ lấy 5 ngày cuối cùng
                .ToList();

            return Ok(totalRevenueByDay);
        }

        [HttpGet("GetAllRevenue")]
        public async Task<ActionResult<decimal>> GetAllRevenue()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var totalRevenue = _context.Orders.Sum(o => o.TotalAmount);

            return Ok(totalRevenue);

        }
        [HttpGet("GetAllShipping")]
        public async Task<ActionResult<object>> GetAllShipping()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            // Tính tổng số đơn hàng có status = 2
            var shippingOrders = await _context.Orders
                .Where(o => o.Status == 2)
                .ToListAsync();

            var shippingOrdersCount = shippingOrders.Count;

            // Tính tổng số tiền của các đơn hàng đang giao
            var totalShippingAmount = shippingOrders.Sum(o => o.TotalAmount); // Giả sử đơn hàng có thuộc tính 'TotalAmount' lưu tổng số tiền

            // Trả về đối tượng gồm tổng số đơn hàng đang giao và tổng số tiền
            var result = new
            {
                TotalOrders = shippingOrdersCount,
                TotalAmount = totalShippingAmount
            };

            return Ok(result);
        }

        [HttpGet("GetAllUnconfirmed")]
        public async Task<ActionResult<object>> GetAllUnconfirmed()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            // Tính tổng số đơn hàng có status = 2
            var shippingOrders = await _context.Orders
                .Where(o => o.Status == 0)
                .ToListAsync();

            var shippingOrdersCount = shippingOrders.Count;

            // Tính tổng số tiền của các đơn hàng đang giao
            var totalShippingAmount = shippingOrders.Sum(o => o.TotalAmount); // Giả sử đơn hàng có thuộc tính 'TotalAmount' lưu tổng số tiền

            // Trả về đối tượng gồm tổng số đơn hàng đang giao và tổng số tiền
            var result = new
            {
                TotalOrders = shippingOrdersCount,
                TotalAmount = totalShippingAmount
            };

            return Ok(result);
        }
    }
}
