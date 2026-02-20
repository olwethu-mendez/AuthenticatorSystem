using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Profile
{
    public class UploadProfilePictureDto
    {
        public required IFormFile ProfilePicture { get; set; }
    }
}
