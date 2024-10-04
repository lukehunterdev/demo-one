using System;
using LhDev.DemoOne.Exceptions;
using LhDev.DemoOne.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LhDev.DemoOne.Attributes;

public class ApiExceptionAttribute : Attribute, IExceptionFilter
{
    private const string JsonContentType = "application/json";

    public async void OnException(ExceptionContext context)
    {
        var ex = context.Exception;
        context.ExceptionHandled = true;

        ReturnException(context.HttpContext, ex);
    }

    internal static async void ReturnException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.ContentType = JsonContentType;

        if (ex is DemoOneWebException dowEx)
        {
            httpContext.Response.StatusCode = dowEx.StatusCode;
            await httpContext.Response.WriteAsync(dowEx.AsGeneralResponse().ToString());
        }
        else if (ex is NotImplementedException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;

            await httpContext.Response.WriteAsync(GeneralResponseExtensions.NotImplementedAsGeneralResponse().ToString());
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsync(ex.AsGeneralResponse().ToString());
        }
    }
}