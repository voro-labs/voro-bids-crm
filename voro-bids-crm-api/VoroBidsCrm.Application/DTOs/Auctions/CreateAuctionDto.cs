using System.ComponentModel.DataAnnotations;

namespace VoroBidsCrm.Application.DTOs.Auctions
{
    public class CreateAuctionDto
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Organization { get; set; } = string.Empty;

        public DateTimeOffset? BiddingDate { get; set; }
        public DateTimeOffset? PublishDate { get; set; }
        
        public Guid? ResponsibleId { get; set; }
    }
}
