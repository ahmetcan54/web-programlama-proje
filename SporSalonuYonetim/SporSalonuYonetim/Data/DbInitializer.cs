using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Migration uygula
            await context.Database.MigrateAsync();

            // Roller
            string[] roleNames = { "Admin", "Uye" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin kullanıcısı
            string adminEmail = "b251210350@sakarya.edu.tr";
            string adminPassword = "sau";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Ad = "Admin",
                    Soyad = "Kullanıcı",
                    KayitTarihi = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Örnek Spor Salonu
            if (!context.SporSalonlari.Any())
            {
                var salon = new SporSalonu
                {
                    Ad = "FitLife Spor Salonu",
                    Adres = "Adapazarı, Sakarya",
                    Telefon = "+90 555 123 4567",
                    Email = "info@fitlife.com",
                    Aciklama = "Modern ekipmanlarla donatılmış profesyonel spor salonu",
                    Aktif = true
                };
                context.SporSalonlari.Add(salon);
                await context.SaveChangesAsync();

                // Örnek Antrenörler
                var antrenorler = new List<Antrenor>
                {
                    new Antrenor
                    {
                        Ad = "Ahmet",
                        Soyad = "Yılmaz",
                        Email = "ahmet.yilmaz@fitlife.com",
                        Telefon = "+90 555 111 2233",
                        UzmanlikAlani = "Kişisel Antrenman, Vücut Geliştirme",
                        DeneyimYili = 8,
                        Biyografi = "Vücut geliştirme ve kişisel antrenman konusunda 8 yıllık deneyime sahip profesyonel antrenör.",
                        Aktif = true,
                        SporSalonuId = salon.Id
                    },
                    new Antrenor
                    {
                        Ad = "Ayşe",
                        Soyad = "Demir",
                        Email = "ayse.demir@fitlife.com",
                        Telefon = "+90 555 222 3344",
                        UzmanlikAlani = "Yoga, Pilates, Esneklik",
                        DeneyimYili = 5,
                        Biyografi = "Yoga ve pilates eğitmeni olarak 5 yıldır grup dersleri veriyor.",
                        Aktif = true,
                        SporSalonuId = salon.Id
                    },
                    new Antrenor
                    {
                        Ad = "Mehmet",
                        Soyad = "Kara",
                        Email = "mehmet.kara@fitlife.com",
                        Telefon = "+90 555 333 4455",
                        UzmanlikAlani = "Crossfit, Fonksiyonel Antrenman",
                        DeneyimYili = 6,
                        Biyografi = "Crossfit ve fonksiyonel antrenman uzmanı.",
                        Aktif = true,
                        SporSalonuId = salon.Id
                    }
                };
                context.Antrenorler.AddRange(antrenorler);
                await context.SaveChangesAsync();

                // Örnek Hizmetler
                var hizmetler = new List<Hizmet>
                {
                    new Hizmet
                    {
                        Ad = "Kişisel Antrenman",
                        Aciklama = "Birebir kişisel antrenman seansı",
                        Sure = 60,
                        Ucret = 200.00m,
                        Kategori = "Kişisel Antrenman",
                        Aktif = true,
                        SporSalonuId = salon.Id,
                        AntrenorId = antrenorler[0].Id
                    },
                    new Hizmet
                    {
                        Ad = "Yoga Dersi",
                        Aciklama = "Grup yoga dersi",
                        Sure = 90,
                        Ucret = 100.00m,
                        Kategori = "Grup Dersi",
                        Aktif = true,
                        SporSalonuId = salon.Id,
                        AntrenorId = antrenorler[1].Id
                    },
                    new Hizmet
                    {
                        Ad = "Crossfit Seansı",
                        Aciklama = "Yüksek yoğunluklu crossfit antrenmanı",
                        Sure = 60,
                        Ucret = 150.00m,
                        Kategori = "Kişisel Antrenman",
                        Aktif = true,
                        SporSalonuId = salon.Id,
                        AntrenorId = antrenorler[2].Id
                    },
                    new Hizmet
                    {
                        Ad = "Beslenme Danışmanlığı",
                        Aciklama = "Kişiselleştirilmiş beslenme programı",
                        Sure = 45,
                        Ucret = 250.00m,
                        Kategori = "Danışmanlık",
                        Aktif = true,
                        SporSalonuId = salon.Id
                    }
                };
                context.Hizmetler.AddRange(hizmetler);
                await context.SaveChangesAsync();

                // Örnek Müsaitlik (Pazartesi-Cuma, 09:00-18:00)
                var musaitlikler = new List<Musaitlik>();
                foreach (var antrenor in antrenorler)
                {
                    for (int gun = 1; gun <= 5; gun++) // Pazartesi-Cuma
                    {
                        musaitlikler.Add(new Musaitlik
                        {
                            AntrenorId = antrenor.Id,
                            Gun = (DayOfWeek)gun,
                            BaslangicSaati = new TimeSpan(9, 0, 0),
                            BitisSaati = new TimeSpan(18, 0, 0),
                            Aktif = true
                        });
                    }
                }
                context.Musaitlikler.AddRange(musaitlikler);
                await context.SaveChangesAsync();
            }

            // Örnek Üye Kullanıcısı
            string uyeEmail = "uye@test.com";
            string uyePassword = "Test123!";

            var uyeUser = await userManager.FindByEmailAsync(uyeEmail);
            if (uyeUser == null)
            {
                uyeUser = new ApplicationUser
                {
                    UserName = uyeEmail,
                    Email = uyeEmail,
                    EmailConfirmed = true,
                    Ad = "Test",
                    Soyad = "Üye",
                    KayitTarihi = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(uyeUser, uyePassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(uyeUser, "Uye");

                    // Üye profili oluştur
                    var uye = new Uye
                    {
                        Ad = "Test",
                        Soyad = "Üye",
                        Email = uyeEmail,
                        Telefon = "+90 555 999 8877",
                        DogumTarihi = new DateTime(1990, 1, 1),
                        Cinsiyet = "Erkek",
                        Boy = 175,
                        Kilo = 75,
                        VucutTipi = "Mezomorf",
                        Hedefler = "Kilo vermek ve kas yapmak",
                        Aktif = true,
                        UserId = uyeUser.Id
                    };
                    context.Uyeler.Add(uye);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}