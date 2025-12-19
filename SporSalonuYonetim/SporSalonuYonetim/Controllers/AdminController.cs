using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;
using SporSalonuYonetim.Models.ViewModels;

namespace SporSalonuYonetim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var model = new DashboardViewModel
            {
                // Genel Ýstatistikler
                ToplamUyeSayisi = await _context.Uyeler.CountAsync(),
                AktifUyeSayisi = await _context.Uyeler.CountAsync(u => u.Aktif),
                ToplamAntrenorSayisi = await _context.Antrenorler.CountAsync(),
                AktifAntrenorSayisi = await _context.Antrenorler.CountAsync(a => a.Aktif),
                ToplamHizmetSayisi = await _context.Hizmetler.CountAsync(h => h.Aktif),

                // Randevu Ýstatistikleri
                BugunkuRandevuSayisi = await _context.Randevular
                    .CountAsync(r => r.RandevuTarihi.Date == today),
                BekleyenRandevuSayisi = await _context.Randevular
                    .CountAsync(r => r.Durum == RandevuDurumu.Beklemede),
                OnaylananRandevuSayisi = await _context.Randevular
                    .CountAsync(r => r.Durum == RandevuDurumu.Onaylandi),
                IptalEdilenRandevuSayisi = await _context.Randevular
                    .CountAsync(r => r.Durum == RandevuDurumu.IptalEdildi),
                BuAyToplamRandevu = await _context.Randevular
                    .CountAsync(r => r.RandevuTarihi >= thisMonth),

                // Finansal Ýstatistikler
                BuAyGelir = await _context.Randevular
                    .Where(r => r.RandevuTarihi >= thisMonth && r.Durum == RandevuDurumu.Tamamlandi)
                    .Include(r => r.Hizmet)
                    .SumAsync(r => r.Hizmet != null ? r.Hizmet.Ucret : 0),

                // Son Randevular
                SonRandevular = await _context.Randevular
                    .Include(r => r.Uye)
                    .Include(r => r.Antrenor)
                    .Include(r => r.Hizmet)
                    .OrderByDescending(r => r.OlusturmaTarihi)
                    .Take(10)
                    .ToListAsync(),

                // Yeni Üyeler
                YeniUyeler = await _context.Uyeler
                    .OrderByDescending(u => u.UyelikBaslangic)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        // GET: Admin/Uyeler
        public async Task<IActionResult> Uyeler()
        {
            var uyeler = await _context.Uyeler
                .Include(u => u.User)
                .OrderBy(u => u.Ad)
                .ToListAsync();

            return View(uyeler);
        }

        // GET: Admin/Antrenorler
        public async Task<IActionResult> Antrenorler()
        {
            var antrenorler = await _context.Antrenorler
                .Include(a => a.SporSalonu)
                .OrderBy(a => a.Ad)
                .ToListAsync();

            return View(antrenorler);
        }

        // GET: Admin/Randevular
        public async Task<IActionResult> Randevular(string durum)
        {
            ViewData["DurumFilter"] = durum;

            var randevular = _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .AsQueryable();

            if (!string.IsNullOrEmpty(durum) && Enum.TryParse<RandevuDurumu>(durum, out var durumEnum))
            {
                randevular = randevular.Where(r => r.Durum == durumEnum);
            }

            return View(await randevular.OrderByDescending(r => r.RandevuTarihi).ToListAsync());
        }

        // POST: Admin/RandevuOnayla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuOnayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            randevu.Durum = RandevuDurumu.Onaylandi;
            randevu.OnaylamaTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Randevu onaylandý: {RandevuId}", id);
            TempData["SuccessMessage"] = "Randevu baþarýyla onaylandý.";

            return RedirectToAction(nameof(Randevular));
        }

        // POST: Admin/RandevuReddet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuReddet(int id, string iptalNedeni)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            randevu.Durum = RandevuDurumu.Reddedildi;
            randevu.IptalNedeni = iptalNedeni ?? "Yönetici tarafýndan reddedildi";
            randevu.IptalTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Randevu reddedildi: {RandevuId}", id);
            TempData["InfoMessage"] = "Randevu reddedildi.";

            return RedirectToAction(nameof(Randevular));
        }

        // GET: Admin/Hizmetler
        public async Task<IActionResult> Hizmetler()
        {
            var hizmetler = await _context.Hizmetler
                .Include(h => h.SporSalonu)
                .Include(h => h.Antrenor)
                .OrderBy(h => h.Kategori)
                .ThenBy(h => h.Ad)
                .ToListAsync();

            return View(hizmetler);
        }

        // GET: Admin/Istatistikler
        public async Task<IActionResult> Istatistikler()
        {
            var model = new
            {
                ToplamUye = await _context.Uyeler.CountAsync(),
                ToplamAntrenor = await _context.Antrenorler.CountAsync(),
                ToplamRandevu = await _context.Randevular.CountAsync(),
                ToplamGelir = await _context.Randevular
                    .Where(r => r.Durum == RandevuDurumu.Tamamlandi)
                    .Include(r => r.Hizmet)
                    .SumAsync(r => r.Hizmet != null ? r.Hizmet.Ucret : 0),

                AylikRandevular = await _context.Randevular
                    .GroupBy(r => new { r.RandevuTarihi.Year, r.RandevuTarihi.Month })
                    .Select(g => new
                    {
                        Yil = g.Key.Year,
                        Ay = g.Key.Month,
                        Sayi = g.Count()
                    })
                    .OrderByDescending(x => x.Yil)
                    .ThenByDescending(x => x.Ay)
                    .Take(12)
                    .ToListAsync(),

                PopulerHizmetler = await _context.Randevular
                    .Include(r => r.Hizmet)
                    .GroupBy(r => r.HizmetId)
                    .Select(g => new
                    {
                        HizmetAdi = g.First().Hizmet!.Ad,
                        Sayi = g.Count()
                    })
                    .OrderByDescending(x => x.Sayi)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
