using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shipments.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shipments.Api.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(
        IConfiguration config,
        UserManager<AppUser> userManager)
    {
        _config = config;
        _userManager = userManager;
    }

    public async Task<string> CreateTokenAsync(AppUser user)
    {
        var jwt = _config.GetSection("Jwt");

        var key = jwt["Key"]
            ?? throw new InvalidOperationException("Jwt:Key missing");

     
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(
                ClaimTypes.Name,
                user.UserName ?? user.Email ?? user.Id
            ),
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            )
        };

        
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        
        var signingKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var creds =
            new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(jwt["ExpiresMinutes"] ?? "120")
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
