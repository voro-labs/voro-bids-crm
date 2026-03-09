using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Entities;

namespace VoroBidsCrm.Domain.Entities
{
    public class InternalRequest : ISoftDeletable, ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;

        public Guid? AuctionId { get; set; }
        public Auction? Auction { get; set; }

        public InternalRequestStatus Status { get; set; } = InternalRequestStatus.Open;

        public Guid RequesterId { get; set; }
        public UserExtension Requester { get; set; } = null!;

        public Guid? AssigneeId { get; set; }
        public UserExtension? Assignee { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
