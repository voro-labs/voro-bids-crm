using VoroBidsCrm.Domain.Enums;

namespace VoroBidsCrm.Application.DTOs.Auctions
{
    public class AuctionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public DateTimeOffset? BiddingDate { get; set; }
        public DateTimeOffset? PublishDate { get; set; }
        public AuctionStatus Status { get; set; }
        public Guid? ResponsibleId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
