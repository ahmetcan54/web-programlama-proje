using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimi.Data;
using SporSalonuYonetimi.Models;

namespace SporSalonuYonetim.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UyelerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UyelerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Uyeler
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUyeler()
        {
            var uyeler = await _context.Uyeler
                .Where(u => u.Aktif)
                .Select(u => new
                {
                    u.Id,
                    u.Ad,
                    u.Soyad,
                    AdSoyad = u.Ad + " " + u.Soyad,
                    u.Email,
                    u.Telefon,
                    u.DogumTarihi,
                    Yas = DateTime.UtcNow.Year - u.DogumTarihi.Year,
                    u.Cinsiyet,
                    u.Boy,
                    u.Kilo,
                    u.VucutKitleIndeksi,
                    u.UyelikBaslangic,
                    u.UyelikBitis,
                    u.Aktif
                })
                .ToListAsync();

            return Ok(uyeler);
        }

        // GET: api/Uyeler/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUye(int id)
        {
            var uye = await _context.Uyeler
                .Include(u => u.Randevular)
                    .ThenInclude(r => r.Antrenor)
                .Include(u => u.Randevular)
                    .ThenInclude(r => r.Hizmet)
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Ad,
                    u.Soyad,
                    AdSoyad = u.Ad + " " + u.Soyad,
                    u.Email,
                    u.Telefon,
                    u.DogumTarihi,
                    Yas = DateTime.UtcNow.Year - u.DogumTarihi.Year,
                    u.Cinsiyet,
                    u.Boy,
                    u.Kilo,
                    u.VucutKitleIndeksi,
                    u.VucutTipi,
                    u.SaglikDurumu,
                    u.Hedefler,
                    u.UyelikBaslangic,
                    u.UyelikBitis,
                    UyelikSuresi = (DateTime.UtcNow - u.UyelikBaslangic).Days,
                    u.Aktif,
                    ToplamRandevu = u.Randevular != null ? u.Randevular.Count : 0,
                    TamamlananRandevu = u.Randevular != null ? u.Randevular.Count(r => r.Durum == RandevuDurumu.Tamamlandi) : 0,
                    Randevular = u.Randevular!.OrderByDescending(r => r.RandevuTarihi).Take(10).Select(r => new
                    {
                        r.Id,
                        r.RandevuTarihi,
                        r.Durum,
                        Antrenor = r.Antrenor != null ? r.Antrenor.Ad + " " + r.Antrenor.Soyad : null,
                        Hizmet = r.Hizmet != null ? r.Hizmet.Ad : null
                    })
                })
                .FirstOrDefaultAsync();

            if (uye == null)
            {
                return NotFound(new { message = "Üye bulunamadı" });
            }

            return Ok(uye);
        }

        // GET: api/Uyeler/{id}/Randevular
        [HttpGet("{id}/Randevular")]
        public async Task<ActionResult<IEnumerable<object>>> GetUyeRandevular(int id)
        {
            var uye = await _context.Uyeler.FindAsync(id);
            if (uye == null)
            {
                return NotFound(new { message = "Üye bulunamadı" });
            }

            var randevular = await _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => r.UyeId == id)
                .OrderByDescending(r => r.RandevuTarihi)
                .Select(r => new
                {
                    r.Id,
                    r.RandevuTarihi,
                    BaslangicSaati = r.BaslangicSaati.ToString(@"hh\:mm"),
                    BitisSaati = r.BitisSaati.ToString(@"hh\:mm"),
                    r.Durum,
                    Antrenor = r.Antrenor != null ? r.Antrenor.Ad + " " + r.Antrenor.Soyad : null,
                    Hizmet = r.Hizmet != null ? r.Hizmet.Ad : null,
                    Ucret = r.Hizmet != null ? r.Hizmet.Ucret : 0
                })
                .ToListAsync();

            return Ok(randevular);
        }

        // GET: api/Uyeler/{id}/Istatistik
        [HttpGet("{id}/Istatistik")]
        public async Task<ActionResult<object>> GetUyeIstatistik(int id)
        {
            var uye = await _context.Uyeler
                .Include(u => u.Randevular)
                    .ThenInclude(r => r.Hizmet)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (uye == null)
            {
                return NotFound(new { message = "Üye bulunamadı" });
            }

            var istatistik = new
            {
                ToplamRandevu = uye.Randevular?.Count ?? 0,
                BekleyenRandevu = uye.Randevular?.Count(r => r.Durum == RandevuDurumu.Beklemede) ?? 0,
                OnaylananRandevu = uye.Randevular?.Count(r => r.Durum == RandevuDurumu.Onaylandi) ?? 0,
                TamamlananRandevu = uye.Randevular?.Count(r => r.Durum == RandevuDurumu.Tamamlandi) ?? 0,
                IptalEdilenRandevu = uye.Randevular?.Count(r => r.Durum == RandevuDurumu.IptalEdildi) ?? 0,
                ToplamHarcama = uye.Randevular?
                    .Where(r => r.Durum == RandevuDurumu.Tamamlandi && r.Hizmet != null)
                    .Sum(r => r.Hizmet!.Ucret) ?? 0,
                UyelikSuresi = (DateTime.UtcNow - uye.UyelikBaslangic).Days,
                AktifDurum = uye.Aktif
            };

            return Ok(istatistik);
        }

        // PUT: api/Uyeler/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUye(int id, Uye uye)
        {
            if (id != uye.Id)
            {
                return BadRequest(new { message = "ID uyuşmazlığı" });
            }

            _context.Entry(uye).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UyeExists(id))
                {
                    return NotFound(new { message = "Üye bulunamadı" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool UyeExists(int id)
        {
            return _context.Uyeler.Any(e => e.Id == id);
        }
    }
}
