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
        [HttpGet]
        [Authorize(Roles =AppRole.Customer)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            var url = "https://localhost:7061/static/Contents/Images/Products/";
            foreach (var product in _context.Products)
            {
                product.Image = url + product.Image;
            }

            return await _context.Products.ToListAsync();
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
        [HttpGet("ProductsByCategory/{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategoryId(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var url = "https://localhost:7061/static/Contents/Images/Products/";
            var list = _context.Products.Where(x => x.ProductCategoryId == id);
            foreach (var product in list)
            {
                product.Image = url + product.Image;
            }

            return await list.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductByCategoryId(int id)
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
          if (_context.Products == null)
          {
              return Problem("Entity set 'BanHangContext.Products'  is null.");
          }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
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
