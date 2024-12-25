using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace MudBlazorWasmUI.Authentication;

public interface IAuthenticationService
{
    Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser);
    Task LogoutAsync();
    Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(RegisterUserModel registerUser);
}
