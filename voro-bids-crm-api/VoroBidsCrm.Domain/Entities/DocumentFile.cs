using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Interfaces.Entities;

namespace VoroBidsCrm.Domain.Entities
{
    public class DocumentFile : ISoftDeletable, ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public Guid AuctionDocumentId { get; set; }
        public AuctionDocument AuctionDocument { get; set; } = null!;

        [StringLength(2048)]
        public string FileUrl { get; set; } = string.Empty;

        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        public int Version { get; set; } = 1;

        public Guid UploadedById { get; set; }
        public UserExtension UploadedBy { get; set; } = null!;

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
