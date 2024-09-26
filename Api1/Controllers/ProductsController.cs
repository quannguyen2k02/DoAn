using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Api1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly BanHangContext _context;

        public ProductsController(BanHangContext context)
        {
            _context = context;
        }

        // GET: api/Products
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        //{
        //    if (_context.Products == null)
        //    {
        //        return NotFound();
        //    }
        //    var url = "https://localhost:7061/static/Contents/Images/Products/";
        //    foreach (var product in _context.Products)
        //    {
        //        product.Image = url + product.Image;
        //    }

        //    return await _context.Products.ToListAsync();
        //}

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 8)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var url = "https://localhost:7061/static/Contents/Images/Products/";

            // Tính tổng số sản phẩm
            var totalProducts = await _context.Products.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Lấy danh sách sản phẩm theo phân trang
            var products = await _context.Products
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();

            // Gắn URL ảnh vào sản phẩm
            foreach (var product in products)
            {
                product.Image = url + product.Image;
            }

            // Tạo đối tượng trả về bao gồm danh sách sản phẩm và số trang
            var result = new
            {
                Products = products,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        [HttpGet("HotProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetHotProducts()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var url = "https://localhost:7061/static/Contents/Images/Products/";
            var list = _context.Products.Where(x => x.isHot == true);
            foreach (var product in list)
            {
                product.Image = url + product.Image;
            }

            return await list.ToListAsync();
        }

        [HttpGet("NewProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> NewProducts()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var url = "https://localhost:7061/static/Contents/Images/Products/";
            var list = _context.Products.OrderByDescending(x => x.Id);
            foreach (var product in list)
            {
                product.Image = url + product.Image;
            }

            return await list.ToListAsync();
        }

        // GET: api/Products/5
        //[HttpGet("ProductsByCategory/{id}")]
        //public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategoryId(int id)
        //{
        //    if (_context.Products == null)
        //    {
        //        return NotFound();
        //    }
        //    var url = "https://localhost:7061/static/Contents/Images/Products/";
        //    var list = _context.Products.Where(x => x.ProductCategoryId == id);
        //    foreach (var product in list)
        //    {
        //        product.Image = url + product.Image;
        //    }

        //    return await list.ToListAsync();
        //}
        [HttpGet("ProductsByCategory/{id}")]
        public async Task<ActionResult> GetProductsByCategoryId(int id, int pageNumber = 1, int pageSize = 8)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var url = "https://localhost:7061/static/Contents/Images/Products/";

            // Lọc sản phẩm theo CategoryId
            var list = _context.Products.Where(x => x.ProductCategoryId == id);

            // Tính tổng số sản phẩm
            var totalProducts = await list.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Tính toán số sản phẩm bỏ qua
            var skip = (pageNumber - 1) * pageSize;

            // Lấy danh sách sản phẩm theo phân trang
            var paginatedList = await list.Skip(skip).Take(pageSize).ToListAsync();

            // Gắn URL ảnh vào sản phẩm
            foreach (var product in paginatedList)
            {
                product.Image = url + product.Image;
            }

            // Tạo đối tượng trả về bao gồm danh sách sản phẩm và số trang
            var result = new
            {
                Products = paginatedList,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }
        [HttpGet("FindByName/{q}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByName(string q, int pageNumber = 1, int pageSize = 8)
        {
            //if (string.IsNullOrEmpty(q))
            //{
            //    return NoContent();
            //}

            //var products = _context.Products.Where(x => x.Title.Contains(q));
            //if(products == null)
            //{
            //    return NoContent();
            //}
            //string url = "https://localhost:7061/static/Contents/Images/Products/";

            //foreach (var product in products)
            //{
            //    product.Image = url + product.Image;
            //}
            //return await products.ToListAsync();
            if (_context.Products == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(q))
            {
                return NoContent();
            }
            var list = _context.Products.Where(x => x.Title.Contains(q));
            if(list == null)
            {
                return NoContent();
            }
            var url = "https://localhost:7061/static/Contents/Images/Products/";

            // Tính tổng số sản phẩm
            var totalProducts = await list.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Lấy danh sách sản phẩm theo phân trang
            var products = await list
                                     .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();

            // Gắn URL ảnh vào sản phẩm
            foreach (var product in products)
            {
                product.Image = url + product.Image;
            }

            // Tạo đối tượng trả về bao gồm danh sách sản phẩm và số trang
            var result = new
            {
                Products = products,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            var url = "https://localhost:7061/static/Contents/Images/Products/";
            product.Image = url + product.Image;
            return product;
        }
        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutProduct( [FromForm] ProductDTO product)
        {
            var productt = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (product == null)
            {
                return BadRequest();
            }
            string image = productt.Image;
            //Xử lý ảnh
            if (product.Image != null)
            {
                var imagePath = Path.Combine("StaticFiles/Contents/Images/Products", product.Image.FileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(stream);
                    image = product.Image.FileName;
                }
            }
            productt.Title = product.Title;
            productt.Description = product.Description;
            productt.Status = product.Status;
            productt.Detail = product.Detail;
            productt.Image = image;
            productt.Price = product.Price;
            productt.PriceSale = product.PriceSale;
            productt.Quantity = product.Quantity;
            productt.isHot = product.isHot;
            productt.ProductCategoryId = product.ProductCategoryId;
            _context.SaveChanges();
            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("UploadFile")]
        public async Task<ActionResult<Product>> PostProduct( [FromForm]ProductDTO product)
        {
          if (_context.Products == null)
          {
              return Problem("Entity set 'BanHangContext.Products'  is null.");
          }
          
          //Xử lý ảnh
          if(product.Image != null)
            {
                var imagePath = Path.Combine("StaticFiles/Contents/Images/Products", product.Image.FileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(stream);
                }
            }
            var productt = new Product
            {
                Title = product.Title,
                Description = product.Description,
                Status = product.Status,
                Detail = product.Detail,
                Image = product.Image.FileName,
                Price = product.Price,
                PriceSale = product.PriceSale,
                Quantity = product.Quantity,
                isHot = product.isHot,
                ProductCategoryId = product.ProductCategoryId
            };
            _context.Products.Add(productt);
            await _context.SaveChangesAsync();
            return StatusCode(201, "successful");
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
