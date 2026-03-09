namespace VoroBidsCrm.Application.DTOs.Auctions
{
    public class AuctionChecklistDto
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
