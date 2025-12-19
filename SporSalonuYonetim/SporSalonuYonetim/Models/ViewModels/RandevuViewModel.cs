using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models.ViewModels
{
    public class RandevuViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Antrenör seçimi zorunludur")]
        [Display(Name = "Antrenör")]
        public int AntrenorId { get; set; }

        [Required(ErrorMessage = "Hizmet seçimi zorunludur")]
        [Display(Name = "Hizmet")]
        public int HizmetId { get; set; }

        [Required(ErrorMessage = "Randevu tarihi zorunludur")]
        [Display(Name = "Randevu Tarihi")]
        [DataType(DataType.Date)]
        public DateTime RandevuTarihi { get; set; }

        [Required(ErrorMessage = "Saat seçimi zorunludur")]
        [Display(Name = "Baþlangýç Saati")]
        public TimeSpan BaslangicSaati { get; set; }

        [Display(Name = "Notunuz")]
        [StringLength(500)]
        public string? UyeNotu { get; set; }

        // Navigation Properties (Display için)
        public string? AntrenorAdi { get; set; }
        public string? HizmetAdi { get; set; }
        public decimal? HizmetUcreti { get; set; }
        public int? HizmetSuresi { get; set; }
        public RandevuDurumu? Durum { get; set; }
        public string? AntrenorNotu { get; set; }
        public TimeSpan? BitisSaati { get; set; }

        // Liste için ek propertyler
        public List<Antrenor>? AntrenorListesi { get; set; }
        public List<Hizmet>? HizmetListesi { get; set; }
        public List<TimeSpan>? UygunSaatler { get; set; }
    }
}