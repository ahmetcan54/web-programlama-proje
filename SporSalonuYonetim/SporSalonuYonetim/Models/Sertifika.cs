using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporSalonuYonetim.Models
{
    public class Sertifika
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Sertifika adı zorunludur")]
        [StringLength(200)]
        public string Ad { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Kurum { get; set; }

        public DateTime? AlisTarihi { get; set; }

        [StringLength(500)]
        public string? SertifikaUrl { get; set; }

        [StringLength(1000)]
        public string? Aciklama { get; set; }

        public bool Aktif { get; set; } = true;

        // Foreign Key
        [Required]
        public int AntrenorId { get; set; }

        // Navigation Property
        [ForeignKey("AntrenorId")]
        public virtual Antrenor? Antrenor { get; set; }
    }
}
