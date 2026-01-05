using WebApi.IdentityLibrary.Entities;

namespace MinimalApi.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}
