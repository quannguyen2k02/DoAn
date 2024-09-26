using Api1.Data;
using Api1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcribesController : Controller
    {
        private readonly BanHangContext _context;
        public SubcribesController(BanHangContext context)
        {
            _context = context;
        }
        [HttpGet("GetAllSubcribe")]
        public async Task<ActionResult<IEnumerable<Subcribe>>> GetSubcribe()
        {
            if (_context.Subcribes == null)
            {
                return NotFound();
            }
            var list = await _context.Subcribes.ToListAsync();
            return Ok(list);
        }
        [HttpPost]
        public async Task<ActionResult> AddSubcribe(Subcribe sub)
        {
            if(sub == null)
            {
                return BadRequest();
            }
            _context.Subcribes.Add(sub);
            _context.SaveChangesAsync();
            return StatusCode(201);
        }
    }
}
