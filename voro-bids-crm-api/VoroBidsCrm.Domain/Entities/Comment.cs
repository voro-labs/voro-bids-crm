using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Interfaces.Entities;

namespace VoroBidsCrm.Domain.Entities
{
    public class Comment : ISoftDeletable, ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public Guid UserId { get; set; }
        public UserExtension User { get; set; } = null!;

        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty; // e.g. "Auction", "InternalRequest"

        public Guid EntityId { get; set; }

        [StringLength(1024)]
        public string Text { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
