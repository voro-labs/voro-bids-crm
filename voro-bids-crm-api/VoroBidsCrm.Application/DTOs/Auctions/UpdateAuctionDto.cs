using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Enums;

namespace VoroBidsCrm.Application.DTOs.Auctions
{
    public class UpdateAuctionDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Organization { get; set; } = string.Empty;

        public DateTimeOffset? BiddingDate { get; set; }
        public DateTimeOffset? PublishDate { get; set; }

        public AuctionStatus Status { get; set; }

        public Guid? ResponsibleId { get; set; }
    }
}
