using Microsoft.AspNetCore.Identity;
using WebApi.IdentityLibrary.Entities;
using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace WebApi.IdentityLibrary.Identity;

public interface IAccountRepository
{
    Task<AppUser?> GetAccountAsync(string username);
    Task<List<AppUser>> GetAccountsAsync();
    Task<AppUser> CreateAccountAsync(RegisterUserModel registerUser);
    Task<AppUser> LoginAsync(LoginUserModel loginUser);
    Task UpdateAccountAsync(AccountUpdateModel updateAccount);
    Task<IdentityResult> DeleteAccountAsync(string requestor, string username);
    Task<bool> SaveAllAsync();
}
