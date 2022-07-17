using DataLibrary.Identity.Models;
using DataLibrary.Models;

namespace BlazorWasmUI.Interfaces;

public interface IAuthenticationService
{
    Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser);
    Task LogoutAsync();
}
