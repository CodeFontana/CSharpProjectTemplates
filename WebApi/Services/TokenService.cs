﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.IdentityLibrary.Entities;
using WebApi.Interfaces;

namespace WebApi.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtLifetimeMinutes;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(config["Authentication:JwtSecurityKey"]);
        _key = new SymmetricSecurityKey(keyBytes);
        _jwtIssuer = config["Authentication:JwtIssuer"];
        _jwtAudience = config["Authentication:JwtAudience"];
        _jwtLifetimeMinutes = int.Parse(config["Authentication:JwtExpiryInMinutes"]);
        _userManager = userManager;
    }

    public async Task<string> CreateTokenAsync(AppUser appUser)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, appUser.UserName),
            new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, appUser.UserName),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtIssuer),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtAudience),
            new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddMinutes(_jwtLifetimeMinutes)).ToUnixTimeSeconds().ToString())
        };

        claims.AddRange(await _userManager.GetClaimsAsync(appUser));

        IList<string> roles = await _userManager.GetRolesAsync(appUser);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        SigningCredentials signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new JwtSecurityToken(new JwtHeader(signingCredentials), new JwtPayload(claims));

        JwtSecurityTokenHandler tokenHandler = new();
        return tokenHandler.WriteToken(token);
    }
}
