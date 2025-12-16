using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimi.Data;
using SporSalonuYonetimi.Models;
using SporSalonuYonetimi.Models.ViewModels;

namespace SporSalonuYonetim.Controllers
{
    [Authorize]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RandevuController> _logger;

        public RandevuController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<RandevuController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Randevu
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var uye = await _context.Uyeler.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (uye == null)
            {
                TempData["ErrorMessage"] = "Üye profili bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var randevular = await _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Include(r => r.Uye)
                .Where(r => r.UyeId == uye.Id)
                .OrderByDescending(r => r.RandevuTarihi)
                .ToListAsync();

            return View(randevular);
        }

        // GET: Randevu/Create
        public async Task<IActionResult> Create()
        {
            var model = new RandevuViewModel
            {
                RandevuTarihi = DateTime.UtcNow.Date.AddDays(1),
                AntrenorListesi = await _context.Antrenorler.Where(a => a.Aktif).ToListAsync(),
                HizmetListesi = await _context.Hizmetler.Where(h => h.Aktif).ToListAsync()
            };

            return View(model);
        }

        // POST: Randevu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RandevuViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var uye = await _context.Uyeler.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (uye == null)
            {
                TempData["ErrorMessage"] = "Üye profili bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var hizmet = await _context.Hizmetler.FindAsync(model.HizmetId);
                if (hizmet == null)
                {
                    ModelState.AddModelError("", "Hizmet bulunamadı.");
                    model.AntrenorListesi = await _context.Antrenorler.Where(a => a.Aktif).ToListAsync();
                    model.HizmetListesi = await _context.Hizmetler.Where(h => h.Aktif).ToListAsync();
                    return View(model);
                }

                var bitisSaati = model.BaslangicSaati.Add(TimeSpan.FromMinutes(hizmet.Sure));

                // Randevu çakışma kontrolü
                var cakismaVar = await _context.Randevular.AnyAsync(r =>
                    r.AntrenorId == model.AntrenorId &&
                    r.RandevuTarihi.Date == model.RandevuTarihi.Date &&
                    r.Durum != RandevuDurumu.IptalEdildi &&
                    ((r.BaslangicSaati < bitisSaati && r.BitisSaati > model.BaslangicSaati)));

                if (cakismaVar)
                {
                    ModelState.AddModelError("", "Seçilen saatte antrenörün başka randevusu var.");
                    model.AntrenorListesi = await _context.Antrenorler.Where(a => a.Aktif).ToListAsync();
                    model.HizmetListesi = await _context.Hizmetler.Where(h => h.Aktif).ToListAsync();
                    return View(model);
                }

                var randevu = new Randevu
                {
                    UyeId = uye.Id,
                    AntrenorId = model.AntrenorId,
                    HizmetId = model.HizmetId,
                    RandevuTarihi = model.RandevuTarihi,
                    BaslangicSaati = model.BaslangicSaati,
                    BitisSaati = bitisSaati,
                    UyeNotu = model.UyeNotu,
                    Durum = RandevuDurumu.Beklemede,
                    OlusturmaTarihi = DateTime.UtcNow
                };

                _context.Randevular.Add(randevu);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Yeni randevu oluşturuldu: Üye {UyeId}, Antrenör {AntrenorId}", uye.Id, model.AntrenorId);
                TempData["SuccessMessage"] = "Randevu talebiniz oluşturuldu. Onay bekleniyor.";
                return RedirectToAction(nameof(Index));
            }

            model.AntrenorListesi = await _context.Antrenorler.Where(a => a.Aktif).ToListAsync();
            model.HizmetListesi = await _context.Hizmetler.Where(h => h.Aktif).ToListAsync();
            return View(model);
        }

        // GET: Randevu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Include(r => r.Uye)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (randevu == null)
            {
                return NotFound();
            }

            // Sadece kendi randevularını görebilir
            var user = await _userManager.GetUserAsync(User);
            var uye = await _context.Uyeler.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (!User.IsInRole("Admin") && randevu.UyeId != uye?.Id)
            {
                return Forbid();
            }

            return View(randevu);
        }

        // POST: Randevu/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string iptalNedeni)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var uye = await _context.Uyeler.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (!User.IsInRole("Admin") && randevu.UyeId != uye?.Id)
            {
                return Forbid();
            }

            randevu.Durum = RandevuDurumu.IptalEdildi;
            randevu.IptalNedeni = iptalNedeni;
            randevu.IptalTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Randevu iptal edildi: {RandevuId}", id);
            TempData["SuccessMessage"] = "Randevu iptal edildi.";

            return RedirectToAction(nameof(Index));
        }

        // Admin: Randevu Onayla
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            randevu.Durum = RandevuDurumu.Onaylandi;
            randevu.OnaylamaTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu onaylandı.";
            return RedirectToAction("Randevular", "Admin");
        }

        // Admin: Randevu Reddet
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string iptalNedeni)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            randevu.Durum = RandevuDurumu.Reddedildi;
            randevu.IptalNedeni = iptalNedeni;
            randevu.IptalTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["InfoMessage"] = "Randevu reddedildi.";
            return RedirectToAction("Randevular", "Admin");
        }
    }
}
