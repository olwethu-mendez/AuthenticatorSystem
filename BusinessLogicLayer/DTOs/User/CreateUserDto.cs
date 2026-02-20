using BusinessLogicLayer.Infrastructure;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.User
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        [RequiredIfValueIs(nameof(Gender), UserGender.Other, ErrorMessage = "Please describe your gender.")]
        public string? GenderSelfDescription { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? Email { get; set; }

        [RegularExpression(@"^\+\d{1,3}$", ErrorMessage = "Country code must be '+' followed by 1-3 digits.")]
        [RequiredIf("PhoneNumber", ErrorMessage = "Country code is required.")]
        public string? CountryCode { get; set; }

        // Matches exactly 9 digits
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Phone number must be exactly 9 digits.")]
        public string? PhoneNumber { get; set; }
        public bool? PrefersEmail { get; set; }
    }
}
