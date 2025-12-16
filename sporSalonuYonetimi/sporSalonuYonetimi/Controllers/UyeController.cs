using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimi.Data;
using SporSalonuYonetimi.Models;


namespace SporSalonuYonetim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UyeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UyeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public UyeController(
            ApplicationDbContext context,
            ILogger<UyeController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Uye
        public async Task<IActionResult> Index(string searchString, bool? aktifFilter)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["AktifFilter"] = aktifFilter;

            var uyeler = _context.Uyeler.AsQueryable();

            // Arama
            if (!string.IsNullOrEmpty(searchString))
            {
                uyeler = uyeler.Where(u =>
                    u.Ad.Contains(searchString) ||
                    u.Soyad.Contains(searchString) ||
                    u.Email.Contains(searchString));
            }

            // Aktiflik filtresi
            if (aktifFilter.HasValue)
            {
                uyeler = uyeler.Where(u => u.Aktif == aktifFilter.Value);
            }

            return View(await uyeler.OrderBy(u => u.Ad).ThenBy(u => u.Soyad).ToListAsync());
        }

        // GET: Uye/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uye = await _context.Uyeler
                .Include(u => u.User)
                .Include(u => u.Randevular)
                    .ThenInclude(r => r.Antrenor)
                .Include(u => u.Randevular)
                    .ThenInclude(r => r.Hizmet)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uye == null)
            {
                return NotFound();
            }

            return View(uye);
        }

        // GET: Uye/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uye = await _context.Uyeler.FindAsync(id);
            if (uye == null)
            {
                return NotFound();
            }

            return View(uye);
        }

        // POST: Uye/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,Email,Telefon,DogumTarihi,Cinsiyet,Boy,Kilo,VucutTipi,SaglikDurumu,Hedefler,UyelikBaslangic,UyelikBitis,Aktif,UserId")] Uye uye)
        {
            if (id != uye.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uye);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Üye güncellendi: {Id} - {Ad} {Soyad}", uye.Id, uye.Ad, uye.Soyad);
                    TempData["SuccessMessage"] = "Üye bilgileri başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UyeExists(uye.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(uye);
        }

        // GET: Uye/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uye = await _context.Uyeler
                .Include(u => u.User)
                .Include(u => u.Randevular)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uye == null)
            {
                return NotFound();
            }

            return View(uye);
        }

        // POST: Uye/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uye = await _context.Uyeler
                .Include(u => u.Randevular)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (uye == null)
            {
                return NotFound();
            }

            // Randevusu varsa soft delete
            if (uye.Randevular != null && uye.Randevular.Any())
            {
                uye.Aktif = false;
                _context.Update(uye);
                TempData["InfoMessage"] = "Üye randevuları olduğu için devre dışı bırakıldı.";
            }
            else
            {
                _context.Uyeler.Remove(uye);
                TempData["SuccessMessage"] = "Üye başarıyla silindi.";
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Üye silindi/devre dışı bırakıldı: {Id}", id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Uye/AIAsistan
        [AllowAnonymous]
        [Authorize(Roles = "Uye")]
        public async Task<IActionResult> AIAsistan()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var uye = await _context.Uyeler
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (uye == null)
            {
                return NotFound();
            }

            return View(uye);
        }

        private bool UyeExists(int id)
        {
            return _context.Uyeler.Any(e => e.Id == id);
        }
    }
}
