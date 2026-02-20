using BusinessLogicLayer.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Authentication
{
    public class RegisterDto
    {
        //public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }

        // Matches + followed by 1 to 3 digits (e.g., +1, +27, +266)
        [RegularExpression(@"^\+\d{1,3}$", ErrorMessage = "Country code must be '+' followed by 1-3 digits.")]
        [RequiredIf("PhoneNumber", ErrorMessage = "Country code is required.")]
        public string? CountryCode { get; set; }

        // Matches exactly 9 digits
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Phone number must be exactly 9 digits.")]
        public string? PhoneNumber { get; set; }
        public bool? PrefersEmail { get; set; }
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
