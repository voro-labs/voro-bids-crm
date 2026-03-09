using System.ComponentModel.DataAnnotations;

namespace VoroBidsCrm.Application.DTOs.Auctions
{
    public class CreateAuctionChecklistDto
    {
        [Required(ErrorMessage = "O identificador da licitação é obrigatório.")]
        public Guid AuctionId { get; set; }

        [Required(ErrorMessage = "O nome da tarefa é obrigatório.")]
        [StringLength(255, ErrorMessage = "O nome da tarefa não pode exceder 255 caracteres.")]
        public string TaskName { get; set; } = string.Empty;
    }
}
