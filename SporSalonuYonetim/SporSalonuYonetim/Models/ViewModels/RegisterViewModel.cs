using System.ComponentModel.DataAnnotations;
using SporSalonuYonetim.Attributes;

namespace SporSalonuYonetim.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        [Display(Name = "Ad")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarasý zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarasý giriniz")]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doðum tarihi zorunludur")]
        [Display(Name = "Doðum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime DogumTarihi { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur")]
        [Display(Name = "Cinsiyet")]
        public string Cinsiyet { get; set; } = string.Empty;

        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasýnda olmalýdýr")]
        [Display(Name = "Boy (cm)")]
        public double? Boy { get; set; }

        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasýnda olmalýdýr")]
        [Display(Name = "Kilo (kg)")]
        public double? Kilo { get; set; }

        [Display(Name = "Vücut Tipi")]
        public string? VucutTipi { get; set; }

        [Display(Name = "Hedefleriniz")]
        [StringLength(500)]
        public string? Hedefler { get; set; }

        [Required(ErrorMessage = "Þifre zorunludur")]
        [StringLength(100, ErrorMessage = "Þifre en az {2} karakter olmalýdýr", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Þifre")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Þifre Tekrar")]
        [Compare("Password", ErrorMessage = "Þifreler eþleþmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Kullaným koþullarýný kabul ediyorum")]
        [MustBeTrue(ErrorMessage = "Kullaným koþullarýný kabul etmelisiniz")]
        public bool AcceptTerms { get; set; }
    }
}
