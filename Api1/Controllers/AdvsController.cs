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
    public class AdvsController : ControllerBase
    {
        private readonly BanHangContext _context;

        public AdvsController(BanHangContext context)
        {
            _context = context;
        }

        // GET: api/Advs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Adv>>> GetAdvs()
        {
          if (_context.Advs == null)
          {
              return NotFound();
          }
            return await _context.Advs.ToListAsync();
        }

        // GET: api/Advs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Adv>> GetAdv(int id)
        {
          if (_context.Advs == null)
          {
              return NotFound();
          }
            var adv = await _context.Advs.FindAsync(id);

            if (adv == null)
            {
                return NotFound();
            }

            return adv;
        }

        // PUT: api/Advs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdv(int id, Adv adv)
        {
            if (id != adv.Id)
            {
                return BadRequest();
            }

            _context.Entry(adv).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvExists(id))
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

        // POST: api/Advs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Adv>> PostAdv(Adv adv)
        {
          if (_context.Advs == null)
          {
              return Problem("Entity set 'BanHangContext.Advs'  is null.");
          }
            _context.Advs.Add(adv);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdv", new { id = adv.Id }, adv);
        }

        // DELETE: api/Advs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdv(int id)
        {
            if (_context.Advs == null)
            {
                return NotFound();
            }
            var adv = await _context.Advs.FindAsync(id);
            if (adv == null)
            {
                return NotFound();
            }

            _context.Advs.Remove(adv);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdvExists(int id)
        {
            return (_context.Advs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
