using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Entities;

namespace VoroBidsCrm.Domain.Entities
{
    public class AuctionDocument : ISoftDeletable, ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public Guid AuctionId { get; set; }
        public Auction Auction { get; set; } = null!;

        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        public bool RequiresUpload { get; set; } = true;

        public Guid? ResponsibleId { get; set; }
        public UserExtension? Responsible { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public ICollection<DocumentFile> Files { get; set; } = [];
    }
}
