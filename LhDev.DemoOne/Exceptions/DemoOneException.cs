using System;

namespace LhDev.DemoOne.Exceptions;

public class DemoOneException : Exception
{
    public int ExitCode { get; init; }


    #region Constructors

    protected DemoOneException(string? message) : base(message)
    {
    }

    protected DemoOneException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    #endregion


    #region Settings errors

    private static DemoOneException NoSettings(string settingsName)
        => new($"Could not find '{settingsName}' settings section. Please check appsettings.json.") { ExitCode = 1 };

    public static DemoOneException NoJwtSettings()
        => NoSettings("Jwt");

    #endregion
}