using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;
using SporSalonuYonetim.Models.ViewModels;

namespace SporSalonuYonetim.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanýcý giriþ yaptý: {Email}", model.Email);

                    // Son giriþ tarihini güncelle
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        user.SonGirisTarihi = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }

                    TempData["SuccessMessage"] = "Hoþ geldiniz!";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email veya þifre hatalý.");
                    TempData["ErrorMessage"] = "Giriþ baþarýsýz. Lütfen bilgilerinizi kontrol edin.";
                }
            }

            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    EmailConfirmed = true,
                    KayitTarihi = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Yeni kullanýcý oluþturuldu: {Email}", model.Email);

                    // Üye rolü ata
                    await _userManager.AddToRoleAsync(user, "Uye");

                    // Üye profili oluþtur
                    var uye = new Uye
                    {
                        Ad = model.Ad,
                        Soyad = model.Soyad,
                        Email = model.Email,
                        Telefon = model.Telefon,
                        DogumTarihi = model.DogumTarihi,
                        Cinsiyet = model.Cinsiyet,
                        Boy = model.Boy,
                        Kilo = model.Kilo,
                        VucutTipi = model.VucutTipi,
                        Hedefler = model.Hedefler,
                        Aktif = true,
                        UserId = user.Id,
                        UyelikBaslangic = DateTime.UtcNow
                    };

                    _context.Uyeler.Add(uye);
                    await _context.SaveChangesAsync();

                    // Otomatik giriþ yap
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["SuccessMessage"] = "Kayýt baþarýlý! Hoþ geldiniz.";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                TempData["ErrorMessage"] = "Kayýt sýrasýnda bir hata oluþtu.";
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Kullanýcý çýkýþ yaptý.");
            TempData["InfoMessage"] = "Baþarýyla çýkýþ yaptýnýz.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var uye = await _context.Uyeler
                .Include(u => u.Randevular)
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (uye == null)
            {
                // Üye kaydý yoksa yeni bir üye oluþtur
                uye = new Uye
                {
                    UserId = user.Id,
                    Ad = user.Ad ?? "",
                    Soyad = user.Soyad ?? "",
                    Email = user.Email ?? "",
                    UyelikBaslangic = DateTime.UtcNow,
                    Aktif = true
                };

                _context.Uyeler.Add(uye);
                await _context.SaveChangesAsync();

                TempData["InfoMessage"] = "Hoþ geldiniz! Lütfen profil bilgilerinizi tamamlayýn.";
            }

            return View(uye);
        }

        // POST: /Account/Profile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var uye = await _context.Uyeler.FirstOrDefaultAsync(u => u.UserId == user.Id);
            if (uye == null)
            {
                return NotFound();
            }

            // Profil bilgilerini güncelle
            uye.Ad = model.Ad;
            uye.Soyad = model.Soyad;
            uye.Telefon = model.Telefon;
            uye.DogumTarihi = model.DogumTarihi ?? uye.DogumTarihi;
            uye.Cinsiyet = model.Cinsiyet ?? uye.Cinsiyet;
            uye.Boy = model.Boy;
            uye.Kilo = model.Kilo;
            uye.VucutTipi = model.VucutTipi;
            uye.SaglikDurumu = model.SaglikDurumu;
            uye.Hedefler = model.Hedefler;

            // ApplicationUser güncelle
            user.Ad = model.Ad;
            user.Soyad = model.Soyad;
            await _userManager.UpdateAsync(user);

            // Þifre deðiþikliði varsa
            if (!string.IsNullOrEmpty(model.MevcutSifre) && !string.IsNullOrEmpty(model.YeniSifre))
            {
                var passwordResult = await _userManager.ChangePasswordAsync(user, model.MevcutSifre, model.YeniSifre);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Profiliniz baþarýyla güncellendi.";
            return RedirectToAction(nameof(Profile));
        }

        // POST: /Account/UpdateProfile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Uye model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var uye = await _context.Uyeler.FirstOrDefaultAsync(u => u.UserId == user.Id);
            if (uye == null)
            {
                return NotFound();
            }

            // Profil bilgilerini güncelle
            uye.Ad = model.Ad;
            uye.Soyad = model.Soyad;
            uye.Telefon = model.Telefon;
            uye.DogumTarihi = model.DogumTarihi;
            uye.Cinsiyet = model.Cinsiyet;
            uye.Boy = model.Boy;
            uye.Kilo = model.Kilo;
            uye.VucutTipi = model.VucutTipi;
            uye.SaglikDurumu = model.SaglikDurumu;
            uye.Hedefler = model.Hedefler;

            // ApplicationUser güncelle
            user.Ad = model.Ad;
            user.Soyad = model.Soyad;
            await _userManager.UpdateAsync(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Kullanýcý profili güncellendi: {UserId}", user.Id);
            TempData["SuccessMessage"] = "Profiliniz baþarýyla güncellendi.";

            return RedirectToAction(nameof(Profile));
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
