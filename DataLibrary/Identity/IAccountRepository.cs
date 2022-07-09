using DataLibrary.Entities;
using DataLibrary.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace DataLibrary.Identity;

public interface IAccountRepository
{
    Task<AppUser> CreateAsync(RegisterUserModel registerUser);
    Task<IdentityResult> DeleteAsync(string requestor, string username);
    Task<AppUser> GetAsync(string username);
    Task<AppUser> LoginAsync(LoginUserModel loginUser);
    Task<bool> SaveAllAsync();
}
