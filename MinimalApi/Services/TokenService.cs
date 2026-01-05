using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebApi.IdentityLibrary.Entities;

namespace MinimalApi.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtLifetimeMinutes;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(config["Authentication:JwtSecurityKey"]
            ?? throw new InvalidOperationException("JWT Security Key not configured"));
        _key = new SymmetricSecurityKey(keyBytes);
        _jwtIssuer = config["Authentication:JwtIssuer"]
            ?? throw new InvalidOperationException("JWT Issuer not configured");
        _jwtAudience = config["Authentication:JwtAudience"]
            ?? throw new InvalidOperationException("JWT Audience not configured");
        _jwtLifetimeMinutes = int.Parse(config["Authentication:JwtExpiryInMinutes"] ?? "60");
        _userManager = userManager;
    }

    public async Task<string> CreateTokenAsync(AppUser appUser)
    {
        if (string.IsNullOrWhiteSpace(appUser.UserName))
        {
            throw new ArgumentException("AppUser must have a valid UserName");
        }

        List<Claim> claims =
        [
            new Claim(ClaimTypes.Name, appUser.UserName),
            new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, appUser.UserName),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtIssuer),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtAudience),
            new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddMinutes(_jwtLifetimeMinutes)).ToUnixTimeSeconds().ToString()),
            .. await _userManager.GetClaimsAsync(appUser),
        ];

        IList<string> roles = await _userManager.GetRolesAsync(appUser);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        SigningCredentials signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new JwtSecurityToken(new JwtHeader(signingCredentials), new JwtPayload(claims));

        JwtSecurityTokenHandler tokenHandler = new();
        return tokenHandler.WriteToken(token);
    }
}
