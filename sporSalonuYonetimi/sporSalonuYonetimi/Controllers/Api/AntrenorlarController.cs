using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimi.Data;
using SporSalonuYonetimi.Models;

namespace SporSalonuYonetim.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AntrenorlerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AntrenorlerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Antrenorler
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAntrenorler()
        {
            var antrenorler = await _context.Antrenorler
                .Include(a => a.SporSalonu)
                .Where(a => a.Aktif)
                .Select(a => new
                {
                    a.Id,
                    a.Ad,
                    a.Soyad,
                    AdSoyad = a.Ad + " " + a.Soyad,
                    a.Email,
                    a.Telefon,
                    a.UzmanlikAlani,
                    a.DeneyimYili,
                    a.Aktif,
                    SporSalonu = a.SporSalonu != null ? a.SporSalonu.Ad : null
                })
                .ToListAsync();

            return Ok(antrenorler);
        }

        // GET: api/Antrenorler/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAntrenor(int id)
        {
            var antrenor = await _context.Antrenorler
                .Include(a => a.SporSalonu)
                .Include(a => a.Randevular)
                .Include(a => a.Hizmetler)
                .Include(a => a.Sertifikalar)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.Ad,
                    a.Soyad,
                    AdSoyad = a.Ad + " " + a.Soyad,
                    a.Email,
                    a.Telefon,
                    a.UzmanlikAlani,
                    a.DeneyimYili,
                    a.Biyografi,
                    Sertifikalar = a.Sertifikalar != null ? a.Sertifikalar.Where(s => s.Aktif).Select(s => new { s.Id, s.Ad, s.Kurum, s.AlisTarihi }) : null,
                    a.Aktif,
                    SporSalonu = a.SporSalonu != null ? a.SporSalonu.Ad : null,
                    ToplamRandevu = a.Randevular != null ? a.Randevular.Count : 0,
                    TamamlananRandevu = a.Randevular != null ? a.Randevular.Count(r => r.Durum == RandevuDurumu.Tamamlandi) : 0,
                    Hizmetler = a.Hizmetler!.Select(h => new { h.Id, h.Ad, h.Ucret })
                })
                .FirstOrDefaultAsync();

            if (antrenor == null)
            {
                return NotFound(new { message = "Antrenör bulunamadı" });
            }

            return Ok(antrenor);
        }

        // POST: api/Antrenorler
        [HttpPost]
        public async Task<ActionResult<Antrenor>> PostAntrenor(Antrenor antrenor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Antrenorler.Add(antrenor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAntrenor), new { id = antrenor.Id }, antrenor);
        }

        // PUT: api/Antrenorler/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAntrenor(int id, Antrenor antrenor)
        {
            if (id != antrenor.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            _context.Entry(antrenor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AntrenorExists(id))
                {
                    return NotFound(new { message = "Antrenör bulunamadı" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Antrenorler/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAntrenor(int id)
        {
            var antrenor = await _context.Antrenorler.FindAsync(id);
            if (antrenor == null)
            {
                return NotFound(new { message = "Antrenör bulunamadı" });
            }

            // Soft delete
            antrenor.Aktif = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Antrenör pasif edildi" });
        }

        // GET: api/Antrenorler/{id}/Randevular
        [HttpGet("{id}/Randevular")]
        public async Task<ActionResult<IEnumerable<object>>> GetAntrenorRandevular(int id)
        {
            var antrenor = await _context.Antrenorler.FindAsync(id);
            if (antrenor == null)
            {
                return NotFound(new { message = "Antrenör bulunamadı" });
            }

            var randevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Hizmet)
                .Where(r => r.AntrenorId == id)
                .OrderByDescending(r => r.RandevuTarihi)
                .Select(r => new
                {
                    r.Id,
                    r.RandevuTarihi,
                    BaslangicSaati = r.BaslangicSaati.ToString(@"hh\:mm"),
                    BitisSaati = r.BitisSaati.ToString(@"hh\:mm"),
                    r.Durum,
                    Uye = r.Uye != null ? r.Uye.Ad + " " + r.Uye.Soyad : null,
                    Hizmet = r.Hizmet != null ? r.Hizmet.Ad : null,
                    Ucret = r.Hizmet != null ? r.Hizmet.Ucret : 0
                })
                .ToListAsync();

            return Ok(randevular);
        }

        private bool AntrenorExists(int id)
        {
            return _context.Antrenorler.Any(e => e.Id == id);
        }
    }
}
