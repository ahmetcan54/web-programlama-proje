using SporSalonuYonetimi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporSalonuYonetimi.Models
{
    public class Uye
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

        [Required]
        public DateTime DogumTarihi { get; set; }

        [Required]
        [StringLength(10)]
        public string Cinsiyet { get; set; } = string.Empty; // Erkek, Kadın, Diğer

        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır")]
        public double? Boy { get; set; } // cm

        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasında olmalıdır")]
        public double? Kilo { get; set; } // kg

        [NotMapped]
        public double? VucutKitleIndeksi
        {
            get
            {
                if (Boy.HasValue && Kilo.HasValue && Boy > 0)
                {
                    double boyMetre = Boy.Value / 100.0;
                    return Math.Round(Kilo.Value / (boyMetre * boyMetre), 2);
                }
                return null;
            }
        }

        [StringLength(50)]
        public string? VucutTipi { get; set; } // Ektomorf, Mezomorf, Endomorf

        [StringLength(50)]
        public string? SaglikDurumu { get; set; }

        [StringLength(500)]
        public string? Hedefler { get; set; }

        public DateTime UyelikBaslangic { get; set; } = DateTime.UtcNow;

        public DateTime? UyelikBitis { get; set; }

        public bool Aktif { get; set; } = true;

        // Foreign Key - Identity User
        public string? UserId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Randevu>? Randevular { get; set; }
    }
}