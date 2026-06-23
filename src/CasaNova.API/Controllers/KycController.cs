using CasaNova.Application.UseCases.KYC;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaNova.API.Controllers;

[ApiController]
[Route("api/kyc")]
[Authorize]
public class KycController : ControllerBase
{
    private readonly IMediator _mediator;
    public KycController(IMediator mediator) => _mediator = mediator;

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromForm] IFormFile document, CancellationToken ct)
    {
        using var stream = document.OpenReadStream();
        var command = new SubmitKycCommand(stream, document.FileName);
        await _mediator.Send(command, ct);
        return Ok(new { message = "KYC enviado correctamente." });
    }
}