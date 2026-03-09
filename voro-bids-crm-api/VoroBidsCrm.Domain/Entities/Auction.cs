using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Entities;

namespace VoroBidsCrm.Domain.Entities
{
    public class Auction : ISoftDeletable, ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(255)]
        public string Organization { get; set; } = string.Empty;

        public DateTimeOffset? BiddingDate { get; set; }
        public DateTimeOffset? PublishDate { get; set; }

        public AuctionStatus Status { get; set; } = AuctionStatus.Draft;

        public Guid? ResponsibleId { get; set; }
        public UserExtension? Responsible { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public ICollection<AuctionDocument> Documents { get; set; } = [];
        public ICollection<InternalRequest> InternalRequests { get; set; } = [];
        public ICollection<AuctionChecklist> Checklists { get; set; } = [];
    }
}
