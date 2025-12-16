using SporSalonuYonetimi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporSalonuYonetimi.Models
{
    public class Hizmet
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur")]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(1000)]
        public string Aciklama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Süre zorunludur")]
        [Range(15, 300, ErrorMessage = "Süre 15-300 dakika arasında olmalıdır")]
        public int Sure { get; set; } // Dakika

        [Required(ErrorMessage = "Ücret zorunludur")]
        [Range(0, 10000, ErrorMessage = "Ücret 0-10000 arasında olmalıdır")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Ucret { get; set; }

        [StringLength(100)]
        public string? Kategori { get; set; } // Kişisel Antrenman, Grup Dersi, Beslenme, vb.

        public bool Aktif { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int SporSalonuId { get; set; }

        public int? AntrenorId { get; set; }

        // Navigation Properties
        [ForeignKey("SporSalonuId")]
        public virtual SporSalonu? SporSalonu { get; set; }

        [ForeignKey("AntrenorId")]
        public virtual Antrenor? Antrenor { get; set; }

        public virtual ICollection<Randevu>? Randevular { get; set; }
    }
}