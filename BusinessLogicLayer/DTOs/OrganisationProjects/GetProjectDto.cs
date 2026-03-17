using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.OrganisationProjects
{
    public class GetProjectDto
    {
        public string? ProjectId { get; set; }
        public string? OrganisationId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ProjectImageUrl { get; set; }
        public string? ProjectImageName { get; set; }
    }
    public class GetProjectsDto
    {
        public string? ProjectId { get; set; }
        public string? OrganisationId { get; set; }
        public string? Name { get; set; }
        public string? ProjectImageUrl { get; set; }
    }
}
