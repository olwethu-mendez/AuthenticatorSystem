using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Profile
{
    public class VerifyCodeDto
    {
        public required string Type { get; set; } // "email" or "phone"
        public required string Code { get; set; }
    }
}
