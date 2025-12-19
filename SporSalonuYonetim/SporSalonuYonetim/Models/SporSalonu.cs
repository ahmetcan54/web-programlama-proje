using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models
{
    public class SporSalonu
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon adı zorunludur")]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres zorunludur")]
        [StringLength(500)]
        public string Adres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [Phone]
        [StringLength(20)]
        public string Telefon { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(1000)]
        public string? Aciklama { get; set; }

        public bool Aktif { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Antrenor>? Antrenorler { get; set; }
        public virtual ICollection<Hizmet>? Hizmetler { get; set; }
    }
}