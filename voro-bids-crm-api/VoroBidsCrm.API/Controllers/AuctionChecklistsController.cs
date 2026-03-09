using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoroBidsCrm.Application.DTOs.Auctions;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.App.Auctions;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.API.Controllers
{
    [Route("api/v{version:version}/[controller]/{auctionId:guid}")]
    [Tags("AuctionChecklists")]
    [ApiController]
    [Authorize]
    public class AuctionChecklistsController(
        IAuctionChecklistService checklistService,
        ICurrentUserService currentUserService) : ControllerBase
    {
        private readonly IAuctionChecklistService _checklistService = checklistService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        [HttpGet]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<AuctionChecklistDto>>>> GetByAuctionId(Guid auctionId)
        {
            var result = await _checklistService.GetByAuctionIdAsync(auctionId);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseViewModel<AuctionChecklistDto>>> Create(Guid auctionId, [FromBody] CreateAuctionChecklistDto createDto)
        {
            var tenantId = _currentUserService.TenantId;
            createDto.AuctionId = auctionId;
            var result = await _checklistService.CreateAsync(createDto, tenantId);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<AuctionChecklistDto>>> Update(Guid id, [FromBody] UpdateAuctionChecklistDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest(ResponseViewModel<AuctionChecklistDto>.Fail("O ID da rota não corresponde ao ID do objeto."));

            var result = await _checklistService.UpdateAsync(id, updateDto);
            return StatusCode(result.Status, result);
        }

        [HttpPatch("{id:guid}/toggle")]
        public async Task<ActionResult<ResponseViewModel<object?>>> ToggleCompletion(Guid id)
        {
            var result = await _checklistService.ToggleCompletionAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<object?>>> Delete(Guid id)
        {
            var result = await _checklistService.DeleteAsync(id);
            return StatusCode(result.Status, result);
        }
    }
}
