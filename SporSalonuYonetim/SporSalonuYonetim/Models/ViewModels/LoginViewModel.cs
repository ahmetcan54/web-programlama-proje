using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þifre zorunludur")]
        [DataType(DataType.Password)]
        [Display(Name = "Þifre")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni Hatýrla")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}