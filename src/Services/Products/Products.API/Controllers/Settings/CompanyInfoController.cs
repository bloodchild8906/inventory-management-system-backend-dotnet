using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Features.Settings.Commands;
using Products.Application.Features.Settings.Queries;
using System.IO;

namespace Products.API.Controllers.Settings
{
    [Route("api/Settings/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyInfoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompanyInfoController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var companyInfo = await _mediator.Send(new GetCompanyInfoQuery());
            return Ok(companyInfo);
        }

        [AllowAnonymous]
        [HttpGet("Logo")]
        public async Task<IActionResult> GetLogo()
        {
            var result = await _mediator.Send(new GetCompanyLogoQuery());
            var rawImage = System.IO.File.ReadAllBytes(result.FilePath);
            Response.ContentType = result.ContentType;
            Response.ContentLength = rawImage.Length;
            return File(rawImage, result.ContentType);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UpdateCompanyInfoCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}
