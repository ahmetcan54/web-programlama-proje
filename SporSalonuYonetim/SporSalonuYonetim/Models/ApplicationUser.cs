using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        public string? Ad { get; set; }

        [StringLength(50)]
        public string? Soyad { get; set; }

        public DateTime? KayitTarihi { get; set; } = DateTime.UtcNow;

        public DateTime? SonGirisTarihi { get; set; }

        // Navigation Property
        public virtual Uye? Uye { get; set; }
    }
}