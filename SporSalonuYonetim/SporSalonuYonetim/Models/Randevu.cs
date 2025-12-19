using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporSalonuYonetim.Models
{
    public enum RandevuDurumu
    {
        Beklemede = 0,
        Onaylandi = 1,
        Reddedildi = 2,
        Tamamlandi = 3,
        IptalEdildi = 4
    }

    public class Randevu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UyeId { get; set; }

        [Required]
        public int AntrenorId { get; set; }

        [Required]
        public int HizmetId { get; set; }

        [Required]
        public DateTime RandevuTarihi { get; set; }

        [Required]
        public TimeSpan BaslangicSaati { get; set; }

        [Required]
        public TimeSpan BitisSaati { get; set; }

        [NotMapped]
        public DateTime TamRandevuBaslangic => RandevuTarihi.Date + BaslangicSaati;

        [NotMapped]
        public DateTime TamRandevuBitis => RandevuTarihi.Date + BitisSaati;

        [Required]
        public RandevuDurumu Durum { get; set; } = RandevuDurumu.Beklemede;

        [StringLength(500)]
        public string? UyeNotu { get; set; }

        [StringLength(500)]
        public string? AntrenorNotu { get; set; }

        [StringLength(500)]
        public string? IptalNedeni { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        public DateTime? OnaylamaTarihi { get; set; }

        public DateTime? IptalTarihi { get; set; }

        // Navigation Properties
        [ForeignKey("UyeId")]
        public virtual Uye? Uye { get; set; }

        [ForeignKey("AntrenorId")]
        public virtual Antrenor? Antrenor { get; set; }

        [ForeignKey("HizmetId")]
        public virtual Hizmet? Hizmet { get; set; }
    }
}