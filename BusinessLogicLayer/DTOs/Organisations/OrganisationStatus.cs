using DataAccessLayer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Organisations
{
    public class OrganisationStatus
    {
        public string Status { get; set; } = StatusValue.Activated;
    }
}
