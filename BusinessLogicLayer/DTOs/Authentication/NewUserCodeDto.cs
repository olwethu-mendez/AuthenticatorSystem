using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Authentication
{
    public class NewUserCodeDto
    {
        public string userId { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
    }
}
