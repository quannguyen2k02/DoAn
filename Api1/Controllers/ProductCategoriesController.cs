using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Api1.Data;

namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly BanHangContext _context;

        public ProductCategoriesController(BanHangContext context)
        {
            _context = context;
        }

        // GET: api/ProductCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
          if (_context.ProductCategories == null)
          {
              return NotFound();
          }
            var url = "https://localhost:7061/static/Contents/Images/ProductCategories/";
            foreach (var category in _context.ProductCategories)
            {
                category.Image = url + category.Image;
            }
            return await _context.ProductCategories.ToListAsync();
        }

        // GET: api/ProductCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        {
          if (_context.ProductCategories == null)
          {
              return NotFound();
          }
            var productCategory = await _context.ProductCategories.FindAsync(id);

            if (productCategory == null)
            {
                return NotFound();
            }
            var url = "https://localhost:7061/static/Contents/Images/ProductCategories/";
            productCategory.Image = url + productCategory.Image;
            return productCategory;
        }

        // PUT: api/ProductCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutProductCategory( [FromForm] ProductCategoryDTO productCategoryDTO)
        {
            var category = _context.ProductCategories.FirstOrDefault(x=>x.Id == productCategoryDTO.Id);
            if (_context.ProductCategories == null)
            {
                return Problem("Entity set 'BanHangContext.ProductCategories'  is null.");
            }
            string image = category.Image;
            if (productCategoryDTO.Image != null)
            {
                var imagePath = Path.Combine("StaticFiles/Contents/Images/ProductCategories", productCategoryDTO.Image.FileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await productCategoryDTO.Image.CopyToAsync(stream);
                    image = productCategoryDTO.Image.FileName;
                }
            }

            category.Title = productCategoryDTO.Title;
            category.Description = productCategoryDTO.Description;
            category.ModifiedDate = DateTime.Now;
            category.Image = image;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/ProductCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("AddCategory")]
        public async Task<ActionResult<ProductCategory>> PostProductCategory([FromForm]ProductCategoryDTO productCategoryDTO)
        {
          if (_context.ProductCategories == null)
          {
              return Problem("Entity set 'BanHangContext.ProductCategories'  is null.");
          }
            string image = "";
          if(productCategoryDTO.Image != null)
            {
                var imagePath = Path.Combine("StaticFiles/Contents/Images/ProductCategories", productCategoryDTO.Image.FileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await productCategoryDTO.Image.CopyToAsync(stream);
                    image = productCategoryDTO.Image.FileName;
                }
            }
            var productCategory = new ProductCategory();
            productCategory.Title = productCategoryDTO.Title;
            productCategory.Description = productCategoryDTO.Description;
            productCategory.CreatedDate = DateTime.Now;
            productCategory.Image = image;
            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductCategory", new { id = productCategory.Id }, productCategory);
        }

        // DELETE: api/ProductCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            if (_context.ProductCategories == null)
            {
                return NotFound();
            }
            var productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductCategoryExists(int id)
        {
            return (_context.ProductCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
