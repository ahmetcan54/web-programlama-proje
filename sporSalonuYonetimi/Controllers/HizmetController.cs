using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimi.Data;
using SporSalonuYonetimi.Models;

namespace SporSalonuYonetim.Controllers
{
    [Authorize]
    public class HizmetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HizmetController> _logger;

        public HizmetController(ApplicationDbContext context, ILogger<HizmetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Hizmet
        public async Task<IActionResult> Index(string searchString, string kategori)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentKategori"] = kategori;

            var hizmetler = _context.Hizmetler
                .Include(h => h.SporSalonu)
                .Include(h => h.Antrenor)
                .Where(h => h.Aktif)
                .AsQueryable();

            // Arama
            if (!string.IsNullOrEmpty(searchString))
            {
                hizmetler = hizmetler.Where(h =>
                    h.Ad.Contains(searchString) ||
                    h.Aciklama.Contains(searchString));
            }

            // Kategori filtresi
            if (!string.IsNullOrEmpty(kategori))
            {
                hizmetler = hizmetler.Where(h => h.Kategori == kategori);
            }

            // Kategorileri ViewBag'e ekle
            ViewBag.Kategoriler = await _context.Hizmetler
                .Where(h => h.Aktif && !string.IsNullOrEmpty(h.Kategori))
                .Select(h => h.Kategori)
                .Distinct()
                .ToListAsync();

            return View(await hizmetler.OrderBy(h => h.Kategori).ThenBy(h => h.Ad).ToListAsync());
        }

        // GET: Hizmet/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler
                .Include(h => h.SporSalonu)
                .Include(h => h.Antrenor)
                .Include(h => h.Randevular)
                    .ThenInclude(r => r.Uye)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        // GET: Hizmet/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad");
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler.Where(a => a.Aktif), "Id", "AdSoyad");
            return View();
        }

        // POST: Hizmet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Ad,Aciklama,Sure,Ucret,Kategori,Aktif,SporSalonuId,AntrenorId")] Hizmet hizmet)
        {
            if (ModelState.IsValid)
            {
                hizmet.OlusturmaTarihi = DateTime.UtcNow;
                _context.Add(hizmet);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Yeni hizmet oluşturuldu: {Ad}", hizmet.Ad);
                TempData["SuccessMessage"] = "Hizmet başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad", hizmet.SporSalonuId);
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler.Where(a => a.Aktif), "Id", "AdSoyad", hizmet.AntrenorId);
            return View(hizmet);
        }

        // GET: Hizmet/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
            {
                return NotFound();
            }

            ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad", hizmet.SporSalonuId);
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler.Where(a => a.Aktif), "Id", "AdSoyad", hizmet.AntrenorId);
            return View(hizmet);
        }

        // POST: Hizmet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Aciklama,Sure,Ucret,Kategori,Aktif,OlusturmaTarihi,SporSalonuId,AntrenorId")] Hizmet hizmet)
        {
            if (id != hizmet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hizmet);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Hizmet güncellendi: {Id} - {Ad}", hizmet.Id, hizmet.Ad);
                    TempData["SuccessMessage"] = "Hizmet başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HizmetExists(hizmet.Id))
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

            ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad", hizmet.SporSalonuId);
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler.Where(a => a.Aktif), "Id", "AdSoyad", hizmet.AntrenorId);
            return View(hizmet);
        }

        // GET: Hizmet/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler
                .Include(h => h.SporSalonu)
                .Include(h => h.Antrenor)
                .Include(h => h.Randevular)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        // POST: Hizmet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.Hizmetler
                .Include(h => h.Randevular)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hizmet == null)
            {
                return NotFound();
            }

            // Randevusu varsa soft delete
            if (hizmet.Randevular != null && hizmet.Randevular.Any())
            {
                hizmet.Aktif = false;
                _context.Update(hizmet);
                TempData["InfoMessage"] = "Hizmet randevuları olduğu için devre dışı bırakıldı.";
            }
            else
            {
                _context.Hizmetler.Remove(hizmet);
                TempData["SuccessMessage"] = "Hizmet başarıyla silindi.";
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Hizmet silindi/devre dışı bırakıldı: {Id}", id);

            return RedirectToAction(nameof(Index));
        }

        private bool HizmetExists(int id)
        {
            return _context.Hizmetler.Any(e => e.Id == id);
        }
    }
}