using VoroBidsCrm.Application.DTOs.Auctions;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.Application.Services.Interfaces.App.Auctions
{
    public interface IAuctionChecklistService
    {
        Task<ResponseViewModel<IEnumerable<AuctionChecklistDto>>> GetByAuctionIdAsync(Guid auctionId);
        Task<ResponseViewModel<AuctionChecklistDto>> CreateAsync(CreateAuctionChecklistDto createDto, Guid tenantId);
        Task<ResponseViewModel<AuctionChecklistDto>> UpdateAsync(Guid id, UpdateAuctionChecklistDto updateDto);
        Task<ResponseViewModel<object?>> ToggleCompletionAsync(Guid id);
        Task<ResponseViewModel<object?>> DeleteAsync(Guid id);
    }
}
