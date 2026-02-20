using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Profile
{
    public class UpdateEmailDto
    {
        public string Password { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;
    }
}
