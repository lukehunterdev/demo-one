namespace LhDev.DemoOne.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool CanEdit { get; set; }
}