namespace DataLibrary.Identity;

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

    public async Task<AppUser> CreateAsync(RegisterUserModel registerUser)
    {
        if (await UserExistsAsync(registerUser.Email))
        {
            throw new ArgumentException($"User is already registered [{registerUser.Email}]");
        }

        AppUser appUser = new()
        {
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
            throw new Exception($"Failed to register user [{appUser.UserName}]");
        }

        return appUser;
    }

    public async Task<AppUser> LoginAsync(LoginUserModel loginUser)
    {
        AppUser appUser = await _userManager.Users
            .SingleOrDefaultAsync(u => u.NormalizedEmail == loginUser.Email.ToUpper());

        if (appUser == null)
        {
            throw new ArgumentException($"Invalid user [{loginUser.Email}]");
        }

        SignInResult result = await _signInManager.CheckPasswordSignInAsync(
                appUser, loginUser.Password, false);

        if (result.Succeeded == false)
        {
            throw new Exception($"Invalid password for user [{loginUser.Email}]");
        }

        return appUser;
    }

    public async Task<AppUser> GetAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username must not be NULL or empty");
        }

        return await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<IdentityResult> DeleteAsync(string requestor, string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username must not be NULL or empty");
        }

        AppUser appUser = await _userManager.Users
            .SingleOrDefaultAsync(u => u.NormalizedEmail == username.ToUpper());

        if (appUser == null)
        {
            throw new Exception("Username not found");
        }

        if (appUser.UserName.ToUpper().Equals(requestor.ToUpper()))
        {
            throw new Exception("Unable to delete your own account");
        }

        return await _userManager.DeleteAsync(appUser);
    }

    private async Task<bool> UserExistsAsync(string username)
    {
        return await _userManager.Users.AnyAsync(u => u.UserName == username);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }
}
