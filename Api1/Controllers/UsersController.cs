using API.Models;
using Api1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api1.Models;
namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly BanHangContext _context;
        public UsersController(BanHangContext context)
        {
            _context = context;
        }
        [HttpGet("GetAllUser")]
        public async Task<ActionResult<IEnumerable<AspNetUser>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var list = await _context.Users.ToListAsync();
            return Ok(list);
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<Product>> GetUserById(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
