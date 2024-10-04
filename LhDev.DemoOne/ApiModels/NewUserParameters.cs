using System.ComponentModel.DataAnnotations;

namespace LhDev.DemoOne.ApiModels;

/// <summary>
/// Used to add a new user to the system.
/// </summary>
public class NewUserParameters
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public bool CanEdit { get; set; } = false;
}