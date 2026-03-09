using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoroBidsCrm.Application.DTOs.CompanyDocuments;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.App.CompanyDocuments;
using VoroBidsCrm.Shared.Constants;
using VoroBidsCrm.Shared.Extensions;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.API.Controllers
{
    [Route("api/v{version:version}/[controller]")]
    [Tags("Company Documents")]
    [ApiController]
    [Authorize]
    public class CompanyDocumentsController(
        ICompanyDocumentService documentService,
        ICurrentUserService currentUserService) : ControllerBase
    {
        private readonly ICompanyDocumentService _documentService = documentService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        [HttpGet]
        public async Task<ActionResult<ResponseViewModel<List<CompanyDocumentDto>>>> GetAll()
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.GetAllAsync(tenantId);
            return StatusCode(response.Status, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<CompanyDocumentDto>>> GetById(Guid id)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.GetByIdAsync(id, tenantId);
            return StatusCode(response.Status, response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Management")]
        public async Task<ActionResult<ResponseViewModel<CompanyDocumentDto>>> Create([FromForm] CreateCompanyDocumentDto dto)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.CreateAsync(tenantId, dto);
            return StatusCode(response.Status, response);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin, Management")]
        public async Task<ActionResult<ResponseViewModel<CompanyDocumentDto>>> Update(Guid id, [FromBody] UpdateCompanyDocumentDto dto)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.UpdateAsync(id, tenantId, dto);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin, Management")]
        public async Task<ActionResult<ResponseViewModel<object?>>> Delete(Guid id)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.DeleteAsync(id, tenantId);
            return StatusCode(response.Status, response);
        }
    }
}
