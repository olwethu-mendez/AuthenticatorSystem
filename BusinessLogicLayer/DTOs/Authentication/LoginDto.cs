using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Authentication
{
    public class LoginDto
    {

        [RegularExpression(@"^\+\d{1,3}$", ErrorMessage = "Country code must be '+' followed by 1-3 digits.")]
        public string? CountryCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
        public bool? StayLoggedIn { get; set; } = false;
    }
}
