using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VoroBidsCrm.Application.DTOs.CompanyDocuments
{
    public class CreateCompanyDocumentDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTimeOffset? ExpirationDate { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
