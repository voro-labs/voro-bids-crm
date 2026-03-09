using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoroBidsCrm.Application.DTOs.AuctionDocuments;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.App.Auctions;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.API.Controllers
{
    [ApiController]
    [Route("api/v{version:version}/[controller]/{auctionId:guid}")]
    [Tags("Auction Documents")]
    [Authorize]
    public class AuctionDocumentsController(
        IAuctionDocumentService documentService,
        ICurrentUserService currentUserService) : ControllerBase
    {
        private readonly IAuctionDocumentService _documentService = documentService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        [HttpGet]
        public async Task<ActionResult<ResponseViewModel<List<AuctionDocumentDto>>>> GetAllByAuction(Guid auctionId)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.GetAllByAuctionAsync(auctionId, tenantId);
            return StatusCode(response.Status, response);
        }

        [HttpPost("requirements")]
        public async Task<ActionResult<ResponseViewModel<AuctionDocumentDto>>> CreateRequirement([FromBody] CreateAuctionDocumentDto dto)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.CreateRequirementAsync(tenantId, dto);
            return StatusCode(response.Status, response);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<ResponseViewModel<DocumentFileDto>>> UploadFile([FromForm] UploadDocumentFileDto dto)
        {
            var tenantId = _currentUserService.TenantId;
            var userId = _currentUserService.UserId;
            var response = await _documentService.UploadFileAsync(tenantId, userId, dto);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("requirements/{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<object?>>> DeleteRequirement(Guid id)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.DeleteRequirementAsync(id, tenantId);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("files/{fileId:guid}")]
        public async Task<ActionResult<ResponseViewModel<object?>>> DeleteFile(Guid fileId)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.DeleteFileAsync(fileId, tenantId);
            return StatusCode(response.Status, response);
        }
    }
}
