using System.ComponentModel.DataAnnotations;

namespace VoroBidsCrm.Application.DTOs.CompanyDocuments
{
    public class UpdateCompanyDocumentDto
    {
        [Required(ErrorMessage = "O nome do documento é obrigatório.")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTimeOffset? ExpirationDate { get; set; }
    }
}
