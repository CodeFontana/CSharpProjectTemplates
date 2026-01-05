using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.IdentityLibrary.Entities;
using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace WebApi.IdentityLibrary.Identity;

public class AccountRepository : IAccountRepository
{
    private readonly IdentityContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountRepository(IdentityContext context,
                             UserManager<AppUser> userManager,
                             SignInManager<AppUser> signInManager)
    {
        _db = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AppUser?> GetAccountAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username must not be null or empty");
        }

        return await _userManager.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u =>
                (!string.IsNullOrWhiteSpace(u.NormalizedEmail)
                    && u.NormalizedEmail.ToUpper() == username.ToUpper())
                || (!string.IsNullOrWhiteSpace(u.UserName)
                    && u.UserName.ToUpper() == username.ToUpper()));
    }

    public async Task<List<AppUser>> GetAccountsAsync()
    {
        return await _userManager.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<AppUser> CreateAccountAsync(RegisterUserModel registerUser)
    {
        if (string.IsNullOrWhiteSpace(registerUser.Username))
        {
            throw new ArgumentException("Username cannot be null or empty");
        }
        else if (string.IsNullOrWhiteSpace(registerUser.Email))
        {
            throw new ArgumentException("Email cannot be null or empty");
        }
        else if (string.IsNullOrWhiteSpace(registerUser.Password))
        {
            throw new ArgumentException("Password cannot be null or empty");
        }

        if (await UserExistsAsync(registerUser.Email))
        {
            throw new ArgumentException($"User is already registered with [{registerUser.Email}]");
        }

        AppUser appUser = new()
        {
            UserName = registerUser.Username,
            Email = registerUser.Email
        };

        IdentityResult result = await _userManager.CreateAsync(appUser, registerUser.Password);

        if (result.Succeeded == false)
        {
            throw new Exception($"Failed to register user [{appUser.Email}] -- {result}");
        }

        IdentityResult roleResult = await _userManager.AddToRoleAsync(appUser, "User");

        if (roleResult.Succeeded == false)
        {
            await _userManager.DeleteAsync(appUser);
            throw new Exception($"Failed to register user [{appUser.Email}]");
        }

        return appUser;
    }

    public async Task<AppUser> LoginAsync(LoginUserModel loginUser)
    {
        if (string.IsNullOrWhiteSpace(loginUser.Username))
        {
            throw new ArgumentException("Username cannot be null or empty");
        }
        else if (string.IsNullOrWhiteSpace(loginUser.Password))
        {
            throw new ArgumentException("Password cannot be null or empty");
        }

        AppUser? appUser = await _userManager.Users
            .SingleOrDefaultAsync(u =>
                (!string.IsNullOrWhiteSpace(u.NormalizedEmail)
                    && u.NormalizedEmail.ToUpper() == loginUser.Username.ToUpper())
                || (!string.IsNullOrWhiteSpace(u.UserName)
                    && u.UserName.ToUpper() == loginUser.Username.ToUpper()))
            ?? throw new ArgumentException($"Invalid user [{loginUser.Username}]");

        SignInResult result = await _signInManager.CheckPasswordSignInAsync(
                appUser, loginUser.Password, false);

        if (result.Succeeded == false)
        {
            throw new Exception($"Invalid password for user [{loginUser.Username}]");
        }

        return appUser;
    }

    public async Task UpdateAccountAsync(AccountUpdateModel updateAccount)
    {
        if (updateAccount is null)
        {
            throw new ArgumentException("Account for update must not be null");
        }
        else if (string.IsNullOrWhiteSpace(updateAccount.UserName))
        {
            throw new ArgumentException("Username cannot be null or empty");
        }
        else if (string.IsNullOrWhiteSpace(updateAccount.Email))
        {
            throw new ArgumentException("Email cannot be null or empty");
        }

        AppUser? user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Id == updateAccount.Id)
            ?? throw new ArgumentException($"Account not found for update [{updateAccount.Id}/{updateAccount.UserName} - {updateAccount.Email}]");

        user.Email = updateAccount.Email;
        user.UserName = updateAccount.UserName;
        await _db.SaveChangesAsync();
    }

    public async Task<IdentityResult> DeleteAccountAsync(string requestor, string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username must not be null or empty");
        }

        AppUser? appUser = await _userManager.Users
            .SingleOrDefaultAsync(u =>
                (!string.IsNullOrWhiteSpace(u.UserName)
                    && u.UserName.ToUpper() == username.ToUpper())
                || (!string.IsNullOrWhiteSpace(u.NormalizedEmail)
                    && u.NormalizedEmail.ToUpper() == username.ToUpper()))
            ?? throw new Exception("Username not found");

        return appUser.UserName!.Equals(requestor, StringComparison.CurrentCultureIgnoreCase)
            ? throw new InvalidOperationException("Unable to delete your own account")
            : await _userManager.DeleteAsync(appUser);
    }

    private async Task<bool> UserExistsAsync(string username)
    {
        return await _userManager.Users.AnyAsync(e => e.NormalizedEmail == username.ToUpper());
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }
}
