using WebApi.IdentityLibrary.Entities;

namespace WebApi.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}
