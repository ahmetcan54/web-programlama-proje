using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporSalonuYonetim.Models
{
    public class Antrenor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50)]
        public string Soyad { get; set; } = string.Empty;

        [NotMapped]
        public string AdSoyad => $"{Ad} {Soyad}";

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [Phone]
        [StringLength(20)]
        public string Telefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur")]
        [StringLength(200)]
        public string UzmanlikAlani { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "Deneyim yılı 0-50 arasında olmalıdır")]
        public int DeneyimYili { get; set; }

        [StringLength(1000)]
        public string? Biyografi { get; set; }

        [StringLength(500)]
        public string? ProfilFotoUrl { get; set; }

        public bool Aktif { get; set; } = true;

        public DateTime IseBaslamaTarihi { get; set; } = DateTime.UtcNow;

        // Foreign Key
        [Required]
        public int SporSalonuId { get; set; }

        // Navigation Properties
        [ForeignKey("SporSalonuId")]
        public virtual SporSalonu? SporSalonu { get; set; }

        public virtual ICollection<Musaitlik>? Musaitlikler { get; set; }
        public virtual ICollection<Randevu>? Randevular { get; set; }
        public virtual ICollection<Hizmet>? Hizmetler { get; set; }
        public virtual ICollection<Sertifika>? Sertifikalar { get; set; }
    }
}
