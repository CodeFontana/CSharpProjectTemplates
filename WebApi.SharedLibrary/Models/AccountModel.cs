namespace WebApi.SharedLibrary.Identity.Models;

public sealed class AccountModel
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastActive { get; set; }
}
