using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models.ViewModels
{
    public class YapayZekaOneriViewModel
    {
        [Required(ErrorMessage = "Boy bilgisi zorunludur")]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasýnda olmalýdýr")]
        [Display(Name = "Boy (cm)")]
        public double Boy { get; set; }

        [Required(ErrorMessage = "Kilo bilgisi zorunludur")]
        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasýnda olmalýdýr")]
        [Display(Name = "Kilo (kg)")]
        public double Kilo { get; set; }

        [Required(ErrorMessage = "Yaþ bilgisi zorunludur")]
        [Range(15, 100, ErrorMessage = "Yaþ 15-100 arasýnda olmalýdýr")]
        [Display(Name = "Yaþ")]
        public int Yas { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur")]
        [Display(Name = "Cinsiyet")]
        public string Cinsiyet { get; set; } = string.Empty;

        [Display(Name = "Vücut Tipi")]
        public string? VucutTipi { get; set; }

        [Required(ErrorMessage = "Hedef seçimi zorunludur")]
        [Display(Name = "Hedef")]
        public string Hedef { get; set; } = string.Empty;

        [Display(Name = "Aktivite Seviyesi")]
        public string? AktiviteSeviyesi { get; set; }

        [Display(Name = "Saðlýk Durumu")]
        [StringLength(500)]
        public string? SaglikDurumu { get; set; }

        // Response Properties
        public double? VucutKitleIndeksi { get; set; }
        public string? VKIKategorisi { get; set; }
        public string? OnerilenVucutTipi { get; set; }
        public List<EgzersizOnerisi>? EgzersizOnerileri { get; set; }
        public BeslenmeProgrami? BeslenmeProgrami { get; set; }
        public string? GenelTavsiyeler { get; set; }
        public string? UyariMesaji { get; set; }
    }

    public class EgzersizOnerisi
    {
        public string Isim { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public int Set { get; set; }
        public int Tekrar { get; set; }
        public int Sure { get; set; } // Dakika
        public string Zorluk { get; set; } = string.Empty; // Kolay, Orta, Zor
        public int Kalori { get; set; }
    }

    public class BeslenmeProgrami
    {
        public int GunlukKalori { get; set; }
        public double Protein { get; set; } // gram
        public double Karbonhidrat { get; set; } // gram
        public double Yag { get; set; } // gram
        public List<string>? OnerilenBesinler { get; set; }
        public List<string>? KacinilmasGerekenler { get; set; }
        public string? Aciklama { get; set; }
    }
}