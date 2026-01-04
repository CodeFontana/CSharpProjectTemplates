using System.ComponentModel.DataAnnotations;

namespace WebApi.SharedLibrary.Models;

public sealed class LoginUserModel
{
    [Required(ErrorMessage = "Please enter your username or email address")]
    [Display(Name = "Username or Email address")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Please enter your password")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }
}
