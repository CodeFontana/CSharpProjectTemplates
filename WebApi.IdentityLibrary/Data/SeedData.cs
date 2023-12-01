using WebApi.IdentityLibrary.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApi.IdentityLibrary.Data;

public class SeedData
{
    private readonly ILogger<SeedData> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public SeedData(ILogger<SeedData> logger,
                    UserManager<AppUser> userManager,
                    RoleManager<AppRole> roleManager)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedUsersAsync()
    {
        try
        {
            if (await _roleManager.Roles.AnyAsync(x => x.Name.Equals("Administrator")) == false)
            {
                List<AppRole> roles =
                [
                    new AppRole { Name = "Administrator" },
                    new AppRole { Name = "User" },
                ];

                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(role);
                }
            }

            if (await _userManager.Users.AnyAsync(x => x.UserName.ToLower().Equals("brian@codefoxtrot.com")) == false)
            {
                AppUser admin = new()
                {
                    UserName = "brian",
                    Email = "brian@codefoxtrot.com"
                };

                await _userManager.CreateAsync(admin, "Passw0rd123!!");
                await _userManager.AddToRolesAsync(admin, new[] { "Administrator", "User" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database seeding");
        }
    }
}
