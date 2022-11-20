using Microsoft.AspNetCore.Identity;

namespace DataLibrary.Entities;

public class AppUser : IdentityUser<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;
}
