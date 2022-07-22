namespace WebApi.Interfaces;

public interface IAccountService
{
    Task<ServiceResponseModel<bool>> DeleteAccount(string requestor, string username);
    Task<ServiceResponseModel<AccountModel>> GetAccount(string requestor, string username);
    Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser);
    Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(string requestor, RegisterUserModel registerUser);
}
