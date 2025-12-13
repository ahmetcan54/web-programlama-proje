using SporSalonuYonetimi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporSalonuYonetimi.Models
{
    public class Musaitlik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AntrenorId { get; set; }

        [Required]
        public DayOfWeek Gun { get; set; } // 0=Pazar, 1=Pazartesi, ...

        [Required]
        public TimeSpan BaslangicSaati { get; set; }

        [Required]
        public TimeSpan BitisSaati { get; set; }

        public bool Aktif { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // Navigation Property
        [ForeignKey("AntrenorId")]
        public virtual Antrenor? Antrenor { get; set; }

        // Müsaitlik kontrolü için helper method

        public bool SaatAraligindaMi(TimeSpan saat)
        {
            return saat >= BaslangicSaati && saat < BitisSaati;
        }
    }
}