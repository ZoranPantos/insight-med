using InsightMed.API.Models;
using InsightMed.Application.Auth.Commands;
using InsightMed.Application.Auth.Models;
using InsightMed.Application.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult> LoginAsync(LoginInputModel input)
    {
        var command = new LoginCommand(input.Email, input.Password);
        string token = await _sender.Send(command);

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterInputModel input)
    {
        var command = new RegisterCommand(input.Email, input.Password);
        await _sender.Send(command);

        return Ok("Registration successful");
    }

    [Authorize]
    [HttpGet("accountInfo/{id}")]
    public async Task<ActionResult<GetAccountInfoQueryResponse>> GetAccountInfoAsync(string id)
    {
        var response = await _sender.Send(new GetAccountInfoQuery(id));
        return Ok(response);
    }

    [Authorize]
    [HttpPost("changePassword")]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordInputModel input)
    {
        var command = new ChangePasswordCommand(input.CurrentPassword, input.NewPassword);
        await _sender.Send(command);

        return Ok("Password change successful");
    }
}