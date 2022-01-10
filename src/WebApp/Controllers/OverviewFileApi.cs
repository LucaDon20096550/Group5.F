using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Areas.Identity.Data;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OverviewFileApi : ControllerBase
    {
        private readonly MyContext _context;

        public OverviewFileApi(MyContext context)
        {
            _context = context;
        }

        // GET: api/OverviewFileApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OverviewFile>>> GetOverviewFiles()
        {
            return await _context.OverviewFiles.ToListAsync();
        }

        // GET: api/OverviewFileApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OverviewFile>> GetOverviewFile(int id)
        {
            var overviewFile = await _context.OverviewFiles.FindAsync(id);

            if (overviewFile == null)
            {
                return NotFound();
            }

            return overviewFile;
        }

        // PUT: api/OverviewFileApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOverviewFile(int id, OverviewFile overviewFile)
        {
            if (id != overviewFile.Id)
            {
                return BadRequest();
            }

            _context.Entry(overviewFile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OverviewFileExists(id))
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

        // POST: api/OverviewFileApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OverviewFile>> PostOverviewFile(OverviewFile overviewFile)
        {
            _context.OverviewFiles.Add(overviewFile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOverviewFile", new { id = overviewFile.Id }, overviewFile);
        }

        // DELETE: api/OverviewFileApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOverviewFile(int id)
        {
            var overviewFile = await _context.OverviewFiles.FindAsync(id);
            if (overviewFile == null)
            {
                return NotFound();
            }

            _context.OverviewFiles.Remove(overviewFile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OverviewFileExists(int id)
        {
            return _context.OverviewFiles.Any(e => e.Id == id);
        }
    }
}
