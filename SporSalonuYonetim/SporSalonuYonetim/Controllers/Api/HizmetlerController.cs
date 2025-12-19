using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HizmetlerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HizmetlerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Hizmetler
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHizmetler([FromQuery] string? kategori = null)
        {
            var query = _context.Hizmetler
                .Include(h => h.SporSalonu)
                .Include(h => h.Antrenor)
                .Where(h => h.Aktif)
                .AsQueryable();

            if (!string.IsNullOrEmpty(kategori))
            {
                query = query.Where(h => h.Kategori == kategori);
            }

            var hizmetler = await query
                .Select(h => new
                {
                    h.Id,
                    h.Ad,
                    h.Aciklama,
                    h.Ucret,
                    h.Sure,
                    h.Kategori,
                    SporSalonu = h.SporSalonu != null ? h.SporSalonu.Ad : null,
                    Antrenor = h.Antrenor != null ? h.Antrenor.Ad + " " + h.Antrenor.Soyad : null,
                    h.Aktif
                })
                .ToListAsync();

            return Ok(hizmetler);
        }

        // GET: api/Hizmetler/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetHizmet(int id)
        {
            var hizmet = await _context.Hizmetler
                .Include(h => h.SporSalonu)
                .Include(h => h.Antrenor)
                .Include(h => h.Randevular)
                .Where(h => h.Id == id)
                .Select(h => new
                {
                    h.Id,
                    h.Ad,
                    h.Aciklama,
                    h.Ucret,
                    h.Sure,
                    h.Kategori,
                    SporSalonu = h.SporSalonu != null ? new { h.SporSalonu.Id, h.SporSalonu.Ad } : null,
                    Antrenor = h.Antrenor != null ? new
                    {
                        h.Antrenor.Id,
                        AdSoyad = h.Antrenor.Ad + " " + h.Antrenor.Soyad,
                        h.Antrenor.UzmanlikAlani
                    } : null,
                    h.Aktif,
                    ToplamKullanim = h.Randevular != null ? h.Randevular.Count : 0,
                    h.OlusturmaTarihi
                })
                .FirstOrDefaultAsync();

            if (hizmet == null)
            {
                return NotFound(new { message = "Hizmet bulunamadı" });
            }

            return Ok(hizmet);
        }

        // GET: api/Hizmetler/Kategoriler
        [HttpGet("Kategoriler")]
        public async Task<ActionResult<IEnumerable<string>>> GetKategoriler()
        {
            var kategoriler = await _context.Hizmetler
                .Where(h => h.Aktif && !string.IsNullOrEmpty(h.Kategori))
                .Select(h => h.Kategori!)
                .Distinct()
                .OrderBy(k => k)
                .ToListAsync();

            return Ok(kategoriler);
        }

        // POST: api/Hizmetler
        [HttpPost]
        public async Task<ActionResult<Hizmet>> PostHizmet(Hizmet hizmet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            hizmet.OlusturmaTarihi = DateTime.UtcNow;
            _context.Hizmetler.Add(hizmet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHizmet), new { id = hizmet.Id }, hizmet);
        }

        // PUT: api/Hizmetler/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHizmet(int id, Hizmet hizmet)
        {
            if (id != hizmet.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            _context.Entry(hizmet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HizmetExists(id))
                {
                    return NotFound(new { message = "Hizmet bulunamadı" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Hizmetler/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHizmet(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
            {
                return NotFound(new { message = "Hizmet bulunamadı" });
            }

            // Soft delete
            hizmet.Aktif = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Hizmet pasif edildi" });
        }

        private bool HizmetExists(int id)
        {
            return _context.Hizmetler.Any(e => e.Id == id);
        }
    }
}
