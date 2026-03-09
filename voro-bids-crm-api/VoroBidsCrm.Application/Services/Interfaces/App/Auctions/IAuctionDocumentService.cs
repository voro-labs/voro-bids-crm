using VoroBidsCrm.Application.DTOs.AuctionDocuments;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.Application.Services.Interfaces.App.Auctions
{
    public interface IAuctionDocumentService
    {
        Task<ResponseViewModel<List<AuctionDocumentDto>>> GetAllByAuctionAsync(Guid auctionId, Guid tenantId, CancellationToken ct = default);
        Task<ResponseViewModel<AuctionDocumentDto>> CreateRequirementAsync(Guid tenantId, CreateAuctionDocumentDto dto, CancellationToken ct = default);
        Task<ResponseViewModel<DocumentFileDto>> UploadFileAsync(Guid tenantId, Guid userId, UploadDocumentFileDto dto, CancellationToken ct = default);
        Task<ResponseViewModel<object?>> DeleteRequirementAsync(Guid id, Guid tenantId, CancellationToken ct = default);
        Task<ResponseViewModel<object?>> DeleteFileAsync(Guid fileId, Guid tenantId, CancellationToken ct = default);
    }
}
