using DataAccessLayer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Authentication
{
    public class CheckApiDto
    {
        public string? Status { get; set; }
        public string? Username { get; set; }
    }
}
