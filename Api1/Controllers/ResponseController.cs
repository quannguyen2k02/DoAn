using Api1.Data;
using Api1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : Controller
    {
        private readonly BanHangContext _context;
        public ResponseController(BanHangContext context)
        {
            _context = context;
        }
        [HttpGet("GetAllResponse")]
        public async Task<ActionResult<IEnumerable<Response>>> GetResponse()
        {
            if (_context.Responses == null)
            {
                return NotFound();
            }
            var list = await _context.Responses.ToListAsync();
            return Ok(list);
        }
        [HttpPost]
        public async Task<ActionResult> AddResponse(Response res)
        {
            if(res == null)
            {
                return BadRequest();
            }
            res.CreatedDate = DateTime.UtcNow;
            res.ModifiedDate = DateTime.UtcNow;
            _context.Responses.Add(res);
            _context.SaveChangesAsync();
            return StatusCode(201);
        }
    }
}
