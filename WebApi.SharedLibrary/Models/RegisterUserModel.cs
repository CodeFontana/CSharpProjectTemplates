using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApi.SharedLibrary.Models;

public sealed class RegisterUserModel
{
    [Required(ErrorMessage = "Please enter a username")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Please enter your email address")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Please enter a password")]
    [MinLength(6, ErrorMessage = "Password must be at least (6) characters")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password")]
    [DisplayName("Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "Password does not match.")]
    public required string ConfirmPassword { get; set; }
}
