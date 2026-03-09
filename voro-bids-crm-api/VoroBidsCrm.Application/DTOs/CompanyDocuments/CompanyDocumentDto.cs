namespace VoroBidsCrm.Application.DTOs.CompanyDocuments
{
    public class CompanyDocumentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public DateTimeOffset? ExpirationDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
