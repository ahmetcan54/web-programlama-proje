using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetimi.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int UyeId { get; set; }

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50)]
        [Display(Name = "Ad")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50)]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [Phone]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; } = string.Empty;

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? DogumTarihi { get; set; }

        [Display(Name = "Cinsiyet")]
        public string? Cinsiyet { get; set; }

        [Range(100, 250)]
        [Display(Name = "Boy (cm)")]
        public double? Boy { get; set; }

        [Range(30, 300)]
        [Display(Name = "Kilo (kg)")]
        public double? Kilo { get; set; }

        [Display(Name = "Vücut Tipi")]
        public string? VucutTipi { get; set; }

        [Display(Name = "Sağlık Durumu")]
        [StringLength(50)]
        public string? SaglikDurumu { get; set; }

        [Display(Name = "Hedefleriniz")]
        [StringLength(500)]
        public string? Hedefler { get; set; }

        // Şifre değiştirme (opsiyonel)
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string? MevcutSifre { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        [StringLength(100, MinimumLength = 3)]
        public string? YeniSifre { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare("YeniSifre", ErrorMessage = "Şifreler eşleşmiyor")]
        public string? YeniSifreTekrar { get; set; }

        // Ek bilgiler
        public DateTime? UyelikBaslangic { get; set; }
        public DateTime? UyelikBitis { get; set; }
        public int ToplamRandevuSayisi { get; set; }
        public int TamamlananRandevuSayisi { get; set; }
        public double? VucutKitleIndeksi { get; set; }
    }
}