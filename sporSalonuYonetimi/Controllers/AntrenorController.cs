using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimi.Data;
using SporSalonuYonetimi.Models;

namespace SporSalonuYonetim.Controllers
{
    [Authorize]
    public class AntrenorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AntrenorController> _logger;

        public AntrenorController(ApplicationDbContext context, ILogger<AntrenorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Antrenor
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["UzmanlikSortParm"] = sortOrder == "uzmanlik" ? "uzmanlik_desc" : "uzmanlik";
            ViewData["DeneyimSortParm"] = sortOrder == "deneyim" ? "deneyim_desc" : "deneyim";

            var antrenorler = _context.Antrenorler
                .Include(a => a.SporSalonu)
                .Where(a => a.Aktif)
                .AsQueryable();

            // Arama
            if (!String.IsNullOrEmpty(searchString))
            {
                antrenorler = antrenorler.Where(a =>
                    a.Ad.Contains(searchString) ||
                    a.Soyad.Contains(searchString) ||
                    a.UzmanlikAlani.Contains(searchString));
            }

            // Sıralama
            antrenorler = sortOrder switch
            {
                "name_desc" => antrenorler.OrderByDescending(a => a.Ad),
                "uzmanlik" => antrenorler.OrderBy(a => a.UzmanlikAlani),
                "uzmanlik_desc" => antrenorler.OrderByDescending(a => a.UzmanlikAlani),
                "deneyim" => antrenorler.OrderBy(a => a.DeneyimYili),
                "deneyim_desc" => antrenorler.OrderByDescending(a => a.DeneyimYili),
                _ => antrenorler.OrderBy(a => a.Ad),
            };

            return View(await antrenorler.ToListAsync());
        }

        // GET: Antrenor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.SporSalonu)
                .Include(a => a.Musaitlikler)
                .Include(a => a.Hizmetler)
                .Include(a => a.Randevular)
                    .ThenInclude(r => r.Uye)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            return View(antrenor);
        }

        // GET: Antrenor/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad");
            return View();
        }

        // POST: Antrenor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Ad,Soyad,Email,Telefon,UzmanlikAlani,DeneyimYili,Biyografi,ProfilFotoUrl,Aktif,SporSalonuId")] Antrenor antrenor)
        {
            if (ModelState.IsValid)
            {
                // Email benzersizlik kontrolü
                var emailExists = await _context.Antrenorler.AnyAsync(a => a.Email == antrenor.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Bu email adresi zaten kayıtlı.");
                    ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad", antrenor.SporSalonuId);
                    return View(antrenor);
                }

                antrenor.IseBaslamaTarihi = DateTime.UtcNow;
                _context.Add(antrenor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Yeni antrenör oluşturuldu: {Ad} {Soyad}", antrenor.Ad, antrenor.Soyad);
                TempData["SuccessMessage"] = "Antrenör başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad", antrenor.SporSalonuId);
            return View(antrenor);
        }

        // GET: Antrenor/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler.FindAsync(id);
            if (antrenor == null)
            {
                return NotFound();
            }

            ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad", antrenor.SporSalonuId);
            return View(antrenor);
        }

        // POST: Antrenor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,Email,Telefon,UzmanlikAlani,DeneyimYili,Biyografi,ProfilFotoUrl,Aktif,IseBaslamaTarihi,SporSalonuId")] Antrenor antrenor)
        {
            if (id != antrenor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Email benzersizlik kontrolü (kendi emaili hariç)
                    var emailExists = await _context.Antrenorler
                        .AnyAsync(a => a.Email == antrenor.Email && a.Id != antrenor.Id);

                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "Bu email adresi zaten kayıtlı.");
                        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad", antrenor.SporSalonuId);
                        return View(antrenor);
                    }

                    _context.Update(antrenor);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Antrenör güncellendi: {Id} - {Ad} {Soyad}", antrenor.Id, antrenor.Ad, antrenor.Soyad);
                    TempData["SuccessMessage"] = "Antrenör bilgileri başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AntrenorExists(antrenor.Id))
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

            ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari.Where(s => s.Aktif), "Id", "Ad", antrenor.SporSalonuId);
            return View(antrenor);
        }

        // GET: Antrenor/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.SporSalonu)
                .Include(a => a.Randevular)
                .Include(a => a.Hizmetler)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            return View(antrenor);
        }

        // POST: Antrenor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var antrenor = await _context.Antrenorler
                .Include(a => a.Randevular)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            // Randevusu varsa soft delete
            if (antrenor.Randevular != null && antrenor.Randevular.Any())
            {
                antrenor.Aktif = false;
                _context.Update(antrenor);
                TempData["InfoMessage"] = "Antrenör randevuları olduğu için devre dışı bırakıldı.";
            }
            else
            {
                _context.Antrenorler.Remove(antrenor);
                TempData["SuccessMessage"] = "Antrenör başarıyla silindi.";
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Antrenör silindi/devre dışı bırakıldı: {Id}", id);

            return RedirectToAction(nameof(Index));
        }

        private bool AntrenorExists(int id)
        {
            return _context.Antrenorler.Any(e => e.Id == id);
        }
    }
}
