using BusinessLogicLayer.Infrastructure;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Profile
{
    public class CreateProfileDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        [RequiredIfValueIs(nameof(Gender), UserGender.Other, ErrorMessage = "Please describe your gender.")]
        public string? GenderSelfDescription { get; set; }
        public bool StayLoggedIn { get; set; } = false;
        public IFormFile? ProfilePicture { get; set; }
    }
}
