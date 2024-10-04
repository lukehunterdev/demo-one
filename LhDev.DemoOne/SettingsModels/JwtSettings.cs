namespace LhDev.DemoOne.SettingsModels;

public interface IJwtSettings
{
    string Issuer { get; }
    string Audience { get; }
    string Key { get; }
    int Duration { get; }
}

public class JwtSettings : IJwtSettings
{
    public string Issuer => "https://demo-one.com/not-real";

    public string Audience => "https://demo-one.com/not-real";

    public string Key { get; set; } = null!;

    public int Duration { get; set; } = 300;
}