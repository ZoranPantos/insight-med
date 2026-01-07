using AutoMapper;
using InsightMed.Application.Auth.Models;
using InsightMed.Application.Auth.Services.Abstractions;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InsightMed.Infrastructure.Auth.Services;

public sealed class AuthService : IAuthService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthService(
        IOptions<JwtOptions> jwtOptions,
        IMapper mapper,
        UserManager<IdentityUser> userManager)
    {
        _jwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedException("Invalid credentials");

        bool result = await _userManager.CheckPasswordAsync(user, password);

        if (!result)
            throw new UnauthorizedException("Invalid credentials");

        var tokenHandler = new JwtSecurityTokenHandler();
        byte[]? key = Encoding.ASCII.GetBytes(_jwtOptions.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
            ]),
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.ExpiresInDays),
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

    public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedException("User does not exist");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidClientDataException(errors);
        }
    }
}