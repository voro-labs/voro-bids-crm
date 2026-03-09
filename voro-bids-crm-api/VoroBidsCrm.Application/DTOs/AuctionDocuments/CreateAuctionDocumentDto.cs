using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VoroBidsCrm.Application.DTOs.AuctionDocuments
{
    public class CreateAuctionDocumentDto
    {
        [Required]
        public Guid AuctionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool RequiresUpload { get; set; } = true;

        public Guid? ResponsibleId { get; set; }
    }

    public class UploadDocumentFileDto
    {
        [Required]
        public Guid AuctionDocumentId { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
