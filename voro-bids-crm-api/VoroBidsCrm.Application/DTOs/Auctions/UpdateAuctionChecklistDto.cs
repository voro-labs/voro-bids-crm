using System.ComponentModel.DataAnnotations;

namespace VoroBidsCrm.Application.DTOs.Auctions
{
    public class UpdateAuctionChecklistDto
    {
        [Required(ErrorMessage = "O identificador da tarefa é obrigatório.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome da tarefa é obrigatório.")]
        [StringLength(255, ErrorMessage = "O nome da tarefa não pode exceder 255 caracteres.")]
        public string TaskName { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}
