using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Authentication
{
    public class ResetPasswordDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
