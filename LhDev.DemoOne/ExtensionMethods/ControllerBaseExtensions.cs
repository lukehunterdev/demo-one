using LhDev.DemoOne.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace LhDev.DemoOne.ExtensionMethods;

public static class ControllerBaseExtensions
{
    public static IActionResult OkGeneralResponse(this ControllerBase cb, string message)
        => cb.Ok(new GeneralResponse { Message = message });
}