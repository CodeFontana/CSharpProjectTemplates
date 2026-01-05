namespace WebApi.SharedLibrary.Identity.Models;

public sealed class AccountUpdateModel
{
    public int Id { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }
}
