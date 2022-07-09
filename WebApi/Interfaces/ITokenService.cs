using DataLibrary.Entities;

namespace WebApi.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}
