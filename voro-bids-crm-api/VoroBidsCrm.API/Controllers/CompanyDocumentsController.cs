using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoroBidsCrm.Application.DTOs.CompanyDocuments;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.App.CompanyDocuments;
using VoroBidsCrm.Shared.Constants;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.API.Controllers
{
    [ApiController]
    [Route("api/v1/tenant/me/company-documents")]
    [Tags("Company Documents")]
    [Authorize]
    public class CompanyDocumentsController : ControllerBase
    {
        private readonly ICompanyDocumentService _documentService;
        private readonly ICurrentUserService _currentUserService;

        public CompanyDocumentsController(
            ICompanyDocumentService documentService,
            ICurrentUserService currentUserService)
        {
            _documentService = documentService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseViewModel<List<CompanyDocumentDto>>>> GetAll(CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.GetAllAsync(tenantId, ct);
            return StatusCode(response.Status, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResponseViewModel<CompanyDocumentDto>>> GetById(Guid id, CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.GetByIdAsync(id, tenantId, ct);
            return StatusCode(response.Status, response);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstant.Admin + "," + RoleConstant.Management)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ResponseViewModel<CompanyDocumentDto>>> Create(
            [FromForm] CreateCompanyDocumentDto dto,
            CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.CreateAsync(tenantId, dto, ct);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = RoleConstant.Admin + "," + RoleConstant.Management)]
        public async Task<ActionResult<ResponseViewModel<object?>>> Delete(Guid id, CancellationToken ct)
        {
            var tenantId = _currentUserService.TenantId;
            var response = await _documentService.DeleteAsync(id, tenantId, ct);
            return StatusCode(response.Status, response);
        }
    }
}
