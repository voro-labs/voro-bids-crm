namespace VoroBidsCrm.Application.DTOs.AuctionDocuments
{
    public class AuctionDocumentDto
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Status { get; set; }
        public bool RequiresUpload { get; set; }
        public Guid? ResponsibleId { get; set; }
        public List<DocumentFileDto> Files { get; set; } = [];
    }

    public class DocumentFileDto
    {
        public Guid Id { get; set; }
        public Guid AuctionDocumentId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int Version { get; set; }
        public Guid UploadedById { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
