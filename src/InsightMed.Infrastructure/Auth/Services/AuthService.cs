using AutoMapper;
using InsightMed.Application.Auth.Models;
using InsightMed.Application.Auth.Services.Abstractions;
using InsightMed.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InsightMed.Infrastructure.Auth.Services;

public sealed class AuthService : IAuthService
{
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        IMapper mapper,
        UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    // TODO: Read Key and Expires from appsettings via options pattern
    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedException("Invalid credentials");

        bool result = await _userManager.CheckPasswordAsync(user, password);

        if (!result)
            throw new UnauthorizedException("Invalid credentials");

        var tokenHandler = new JwtSecurityTokenHandler();
        byte[]? key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "default_secret_key_default_secret_key_default_secret_key");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
            ]),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task RegisterAsync(string email, string password)
    {
        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidClientDataException(errors);
        }
    }

    public async Task<IdentityUserResponse?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user is null ? null : _mapper.Map<IdentityUserResponse>(user);
    }
}