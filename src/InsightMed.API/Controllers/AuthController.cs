using InsightMed.API.Models;
using InsightMed.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync(LoginRequestInputModel input)
    {
        var command = new LoginCommand(input.Email, input.Password);
        string token = await _sender.Send(command);

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterRequestInputModel input)
    {
        var command = new RegisterCommand(input.Email, input.Password);
        await _sender.Send(command);

        return Ok("Registration successful");
    }
}