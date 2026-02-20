using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Profile
{
    public class UpdatePhoneNumberDto
    {
        public string Password { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string NewPhoneNumber { get; set; } = string.Empty;
    }
}
