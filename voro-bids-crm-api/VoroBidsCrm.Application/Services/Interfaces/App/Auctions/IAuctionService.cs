using VoroBidsCrm.Application.DTOs.Auctions;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.Application.Services.Interfaces.App.Auctions
{
    public interface IAuctionService
    {
        Task<ResponseViewModel<IEnumerable<AuctionDto>>> GetAllAsync();
        Task<ResponseViewModel<AuctionDto>> GetByIdAsync(Guid id);
        Task<ResponseViewModel<AuctionDto>> CreateAsync(CreateAuctionDto createDto);
        Task<ResponseViewModel<AuctionDto>> UpdateAsync(Guid id, UpdateAuctionDto updateDto);
        Task<ResponseViewModel<object?>> DeleteAsync(Guid id);
    }
}
