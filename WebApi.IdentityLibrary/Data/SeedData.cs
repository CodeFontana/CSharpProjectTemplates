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
            if (_roleManager.Roles != null
                && await _roleManager.Roles.AnyAsync() == false)
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

            if (_userManager.Users != null
                && await _userManager.Users.AnyAsync() == false)
            {
                AppUser admin = new()
                {
                    UserName = "brian",
                    Email = "brian@codefoxtrot.com"
                };

                await _userManager.CreateAsync(admin, "Passw0rd123!!");
                await _userManager.AddToRolesAsync(admin, ["Administrator", "User"]);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database seeding");
        }
    }
}
