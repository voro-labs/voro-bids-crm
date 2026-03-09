using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Interfaces.Entities;

namespace VoroBidsCrm.Domain.Entities
{
    public class AuctionChecklist : ISoftDeletable, ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public Guid AuctionId { get; set; }
        public Auction Auction { get; set; } = null!;

        [StringLength(255)]
        public string TaskName { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
