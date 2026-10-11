using System.ComponentModel.DataAnnotations;

namespace FluentUI.Models;

public sealed class FormDemoModel
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Range(0, 130)]
    public int? Age { get; set; }

    public string? Notes { get; set; }

    public bool Subscribe { get; set; }

    public bool Notifications { get; set; }

    public string? ContactMethod { get; set; }

    public string? Color { get; set; }
}
