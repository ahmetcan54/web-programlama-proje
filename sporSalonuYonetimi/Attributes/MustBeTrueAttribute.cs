using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Attributes
{
    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is bool b && b;
        }
    }
}