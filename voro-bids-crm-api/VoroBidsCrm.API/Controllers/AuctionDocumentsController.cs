using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoroBidsCrm.Application.DTOs.AuctionDocuments;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.App.Auctions;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.API.Controllers
{
    [ApiController]
    [Route("api/v{version:version}/auctions/{auctionId:guid}/documents")]
    [Tags("Auction Documents")]
    [Authorize]
    public class AuctionDocumentsController : ControllerBase
    {
        private readonly IAuctionDocumentService _documentService;
        private readonly ICurrentUserService _currentUserService;

        public AuctionDocumentsController(
            IAuctionDocumentService documentService,
            ICurrentUserService currentUserService)
        {
            _documentService = documentService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseViewModel<List<AuctionDocumentDto>>>> GetAllByAuction(Guid auctionId, CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.GetAllByAuctionAsync(auctionId, tenantId, ct);
            return StatusCode(response.Status, response);
        }

        [HttpPost("requirements")]
        public async Task<ActionResult<ResponseViewModel<AuctionDocumentDto>>> CreateRequirement([FromBody] CreateAuctionDocumentDto dto, CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.CreateRequirementAsync(tenantId, dto, ct);
            return StatusCode(response.Status, response);
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ResponseViewModel<DocumentFileDto>>> UploadFile([FromForm] UploadDocumentFileDto dto, CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var userId = _currentUserService.UserId;
            var response = await _documentService.UploadFileAsync(tenantId, userId, dto, ct);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("requirements/{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<object?>>> DeleteRequirement(Guid id, CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.DeleteRequirementAsync(id, tenantId, ct);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("files/{fileId:guid}")]
        public async Task<ActionResult<ResponseViewModel<object?>>> DeleteFile(Guid fileId, CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.DeleteFileAsync(fileId, tenantId, ct);
            return StatusCode(response.Status, response);
        }
    }
}
