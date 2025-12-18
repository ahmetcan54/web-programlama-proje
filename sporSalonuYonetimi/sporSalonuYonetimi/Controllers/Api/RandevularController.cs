using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimi.Data;
using SporSalonuYonetimi.Models;

namespace SporSalonuYonetim.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandevularController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RandevularController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Randevular
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRandevular(
            [FromQuery] RandevuDurumu? durum = null,
            [FromQuery] DateTime? baslangicTarihi = null,
            [FromQuery] DateTime? bitisTarihi = null)
        {
            var query = _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .AsQueryable();

            if (durum.HasValue)
            {
                query = query.Where(r => r.Durum == durum.Value);
            }

            if (baslangicTarihi.HasValue)
            {
                query = query.Where(r => r.RandevuTarihi >= baslangicTarihi.Value);
            }

            if (bitisTarihi.HasValue)
            {
                query = query.Where(r => r.RandevuTarihi <= bitisTarihi.Value);
            }

            var randevular = await query
                .OrderByDescending(r => r.RandevuTarihi)
                .Select(r => new
                {
                    r.Id,
                    r.RandevuTarihi,
                    BaslangicSaati = r.BaslangicSaati.ToString(@"hh\:mm"),
                    BitisSaati = r.BitisSaati.ToString(@"hh\:mm"),
                    r.Durum,
                    r.UyeNotu,
                    r.AntrenorNotu,
                    Uye = r.Uye != null ? new { r.Uye.Id, AdSoyad = r.Uye.Ad + " " + r.Uye.Soyad, r.Uye.Email } : null,
                    Antrenor = r.Antrenor != null ? new { r.Antrenor.Id, AdSoyad = r.Antrenor.Ad + " " + r.Antrenor.Soyad } : null,
                    Hizmet = r.Hizmet != null ? new { r.Hizmet.Id, r.Hizmet.Ad, r.Hizmet.Ucret, r.Hizmet.Sure } : null,
                    r.OlusturmaTarihi
                })
                .ToListAsync();

            return Ok(randevular);
        }

        // GET: api/Randevular/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetRandevu(int id)
        {
            var randevu = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.RandevuTarihi,
                    BaslangicSaati = r.BaslangicSaati.ToString(@"hh\:mm"),
                    BitisSaati = r.BitisSaati.ToString(@"hh\:mm"),
                    r.Durum,
                    r.UyeNotu,
                    r.AntrenorNotu,
                    r.IptalNedeni,
                    Uye = r.Uye != null ? new { r.Uye.Id, AdSoyad = r.Uye.Ad + " " + r.Uye.Soyad, r.Uye.Email, r.Uye.Telefon } : null,
                    Antrenor = r.Antrenor != null ? new { r.Antrenor.Id, AdSoyad = r.Antrenor.Ad + " " + r.Antrenor.Soyad, r.Antrenor.UzmanlikAlani } : null,
                    Hizmet = r.Hizmet != null ? new { r.Hizmet.Id, r.Hizmet.Ad, r.Hizmet.Ucret, r.Hizmet.Sure, r.Hizmet.Aciklama } : null,
                    r.OlusturmaTarihi,
                    r.OnaylamaTarihi,
                    r.IptalTarihi
                })
                .FirstOrDefaultAsync();

            if (randevu == null)
            {
                return NotFound(new { message = "Randevu bulunamadı" });
            }

            return Ok(randevu);
        }

        // POST: api/Randevular
        [HttpPost]
        public async Task<ActionResult<object>> PostRandevu(RandevuCreateDto dto)
        {
            // Çakışma kontrolü
            var cakismaVar = await _context.Randevular
                .AnyAsync(r => r.AntrenorId == dto.AntrenorId &&
                              r.RandevuTarihi.Date == dto.RandevuTarihi.Date &&
                              r.BaslangicSaati < dto.BitisSaati &&
                              r.BitisSaati > dto.BaslangicSaati &&
                              r.Durum != RandevuDurumu.IptalEdildi &&
                              r.Durum != RandevuDurumu.Reddedildi);

            if (cakismaVar)
            {
                return BadRequest(new { message = "Bu saatte antrenörün başka bir randevusu var" });
            }

            var randevu = new Randevu
            {
                UyeId = dto.UyeId,
                AntrenorId = dto.AntrenorId,
                HizmetId = dto.HizmetId,
                RandevuTarihi = dto.RandevuTarihi,
                BaslangicSaati = dto.BaslangicSaati,
                BitisSaati = dto.BitisSaati,
                UyeNotu = dto.UyeNotu,
                Durum = RandevuDurumu.Beklemede,
                OlusturmaTarihi = DateTime.UtcNow
            };

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRandevu), new { id = randevu.Id }, new { message = "Randevu oluşturuldu", randevuId = randevu.Id });
        }

        // PUT: api/Randevular/5/Onayla
        [HttpPut("{id}/Onayla")]
        public async Task<IActionResult> OnaylaRandevu(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound(new { message = "Randevu bulunamadı" });
            }

            if (randevu.Durum != RandevuDurumu.Beklemede)
            {
                return BadRequest(new { message = "Sadece bekleyen randevular onaylanabilir" });
            }

            randevu.Durum = RandevuDurumu.Onaylandi;
            randevu.OnaylamaTarihi = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Randevu onaylandı" });
        }

        // PUT: api/Randevular/5/Iptal
        [HttpPut("{id}/Iptal")]
        public async Task<IActionResult> IptalRandevu(int id, [FromBody] string? iptalNedeni)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound(new { message = "Randevu bulunamadı" });
            }

            randevu.Durum = RandevuDurumu.IptalEdildi;
            randevu.IptalNedeni = iptalNedeni;
            randevu.IptalTarihi = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Randevu iptal edildi" });
        }

        // GET: api/Randevular/MusaitSaatler
        [HttpGet("MusaitSaatler")]
        public async Task<ActionResult<IEnumerable<string>>> GetMusaitSaatler(
            [FromQuery] int antrenorId,
            [FromQuery] DateTime tarih)
        {
            var mevcutRandevular = await _context.Randevular
                .Where(r => r.AntrenorId == antrenorId &&
                           r.RandevuTarihi.Date == tarih.Date &&
                           r.Durum != RandevuDurumu.IptalEdildi &&
                           r.Durum != RandevuDurumu.Reddedildi)
                .Select(r => new { r.BaslangicSaati, r.BitisSaati })
                .ToListAsync();

            var tumSaatler = new List<TimeSpan>();
            for (int saat = 9; saat <= 20; saat++)
            {
                tumSaatler.Add(new TimeSpan(saat, 0, 0));
            }

            var musaitSaatler = tumSaatler.Where(saat =>
            {
                var bitisSaati = saat.Add(TimeSpan.FromHours(1));
                return !mevcutRandevular.Any(r =>
                    saat < r.BitisSaati && bitisSaati > r.BaslangicSaati);
            }).Select(s => s.ToString(@"hh\:mm")).ToList();

            return Ok(musaitSaatler);
        }
    }

    public class RandevuCreateDto
    {
        public int UyeId { get; set; }
        public int AntrenorId { get; set; }
        public int HizmetId { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public TimeSpan BaslangicSaati { get; set; }
        public TimeSpan BitisSaati { get; set; }
        public string? UyeNotu { get; set; }
    }
}
