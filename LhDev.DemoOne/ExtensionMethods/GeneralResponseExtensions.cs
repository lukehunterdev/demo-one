using System;
using LhDev.DemoOne.ApiModels;
using LhDev.DemoOne.Exceptions;
using Microsoft.AspNetCore.Http;

namespace LhDev.DemoOne.ExtensionMethods;

public static class GeneralResponseExtensions
{
    public static GeneralResponse NotImplementedAsGeneralResponse() => new()
    {
        StatusCode = StatusCodes.Status501NotImplemented,
        Message = "This feature is not implemented.",
        Type = "Error",
    };

    public static GeneralResponse AsGeneralResponse(this Exception ex) => new()
    {
        StatusCode = StatusCodes.Status500InternalServerError,
        Message = ex.ToString(),
        Type = "Error",
    };

    public static GeneralResponse AsGeneralResponse(this DemoOneWebException ex) => new()
    {
        StatusCode = ex.StatusCode,
        Message = ex.Message,
        Type = "Error",
    };
}