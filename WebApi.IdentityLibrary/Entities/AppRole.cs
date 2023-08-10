using Microsoft.AspNetCore.Identity;

namespace WebApi.IdentityLibrary.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}
