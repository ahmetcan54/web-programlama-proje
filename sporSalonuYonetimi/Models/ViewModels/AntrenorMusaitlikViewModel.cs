using SporSalonuYonetimi.Models;
using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetimi.Models.ViewModels
{
    public class AntrenorMusaitlikViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Antrenör seçimi zorunludur")]
        [Display(Name = "Antrenör")]
        public int AntrenorId { get; set; }

        [Required(ErrorMessage = "Gün seçimi zorunludur")]
        [Display(Name = "Gün")]
        public DayOfWeek Gun { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan BaslangicSaati { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan BitisSaati { get; set; }

        [Display(Name = "Aktif")]
        public bool Aktif { get; set; } = true;

        // Navigation
        public string? AntrenorAdi { get; set; }
        public List<Antrenor>? AntrenorListesi { get; set; }

        // Helper method
        public string GunAdi
        {
            get
            {
                return Gun switch
                {
                    DayOfWeek.Monday => "Pazartesi",
                    DayOfWeek.Tuesday => "Salı",
                    DayOfWeek.Wednesday => "Çarşamba",
                    DayOfWeek.Thursday => "Perşembe",
                    DayOfWeek.Friday => "Cuma",
                    DayOfWeek.Saturday => "Cumartesi",
                    DayOfWeek.Sunday => "Pazar",
                    _ => Gun.ToString()
                };
            }
        }
    }
}