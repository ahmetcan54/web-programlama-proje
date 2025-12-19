using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;
using SporSalonuYonetim.Models.ViewModels;
using System.Diagnostics;

namespace SporSalonuYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                // En popüler/deneyimli 4 antrenörü getir
                Antrenorler = await _context.Antrenorler
                    .Where(a => a.Aktif)
                    .OrderByDescending(a => a.DeneyimYili)
                    .Take(4)
                    .ToListAsync(),

                // Ýstatistikler 
                ToplamUyeSayisi = await _context.Uyeler.CountAsync(u => u.Aktif),
                AktifAntrenorSayisi = await _context.Antrenorler.CountAsync(a => a.Aktif),
                ToplamHizmetSayisi = await _context.Hizmetler.CountAsync(h => h.Aktif),

                // Galeri resimleri (örnek)
                GaleriResimleri = new List<SalonGaleri>
                {
                    new SalonGaleri { BaslikUrl = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=800", Aciklama = "Modern Fitness Alanýmýz" },
                    new SalonGaleri { BaslikUrl = "https://images.unsplash.com/photo-1571902943202-507ec2618e8f?w=800", Aciklama = "Cardio Bölümümüz" },
                    new SalonGaleri { BaslikUrl = "https://images.unsplash.com/photo-1593476087123-36d1de271f08?w=800", Aciklama = "Aðýrlýk Antrenman Alaný" },
                    new SalonGaleri { BaslikUrl = "https://images.unsplash.com/photo-1623874514711-0f321325f318?w=800", Aciklama = "Grup Ders Stüdyosu" }
                }
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
