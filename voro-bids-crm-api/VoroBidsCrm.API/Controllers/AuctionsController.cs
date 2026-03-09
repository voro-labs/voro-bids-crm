using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoroBidsCrm.Application.DTOs.Auctions;
using VoroBidsCrm.Application.Services.Interfaces.App.Auctions;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.API.Controllers
{
    [Route("api/v{version:version}/[controller]")]
    [Tags("Auctions")]
    [ApiController]
    [Authorize]
    public class AuctionsController(IAuctionService auctionService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<AuctionDto>>>> GetAll()
        {
            var result = await auctionService.GetAllAsync();
            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<AuctionDto>>> GetById(Guid id)
        {
            var result = await auctionService.GetByIdAsync(id);
            if (result.HasError)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseViewModel<AuctionDto>>> Create([FromBody] CreateAuctionDto createDto)
        {
            var result = await auctionService.CreateAsync(createDto);
            if (result.HasError)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<AuctionDto>>> Update(Guid id, [FromBody] UpdateAuctionDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest(ResponseViewModel<AuctionDto>.Fail("O ID da rota não corresponde ao ID do objeto."));

            var result = await auctionService.UpdateAsync(id, updateDto);
            if (result.HasError)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<object?>>> Delete(Guid id)
        {
            var result = await auctionService.DeleteAsync(id);
            if (result.HasError)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
