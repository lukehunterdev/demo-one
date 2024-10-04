using System;
using LhDev.DemoOne.Exceptions;
using LhDev.DemoOne.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LhDev.DemoOne.Attributes;

public abstract class JwtAuthoriseAttribute : Attribute, IAuthorizationFilter
{
    public bool RequiresEdit { get; set; }
    
    protected abstract string Type { get; }

    public virtual DemoOneWebException NotAuthorised(string message = null!)
        => DemoOneWebException.NotAuthorised(message);

    protected void HandleResponse(string? message, AuthorizationFilterContext context)
    {
        ApiExceptionAttribute.ReturnException(context.HttpContext, DemoOneWebException.NotAuthorised(message));
    }

    public virtual void OnAuthorization(AuthorizationFilterContext context)
    {
        var source = (string?)context.HttpContext.Items["JwtSource"];

        if (string.IsNullOrEmpty(source)) throw NotAuthorised("No token found.");
        if (source != Type) throw NotAuthorised();
    
        // If a User object is attached to the context, we have a valid token.
        var user = (User?)context.HttpContext.Items["User"];
        if (user != null)
        {
            // Check the user can edit the resource!
            if (RequiresEdit && !user.CanEdit) 
                throw NotAuthorised("You are not allowed to create, modify, or remove this resource.");

            return;
        }
    
        // If there was an error authenticating the client, throw an exception and let the middleware handle the response cleanly.
        var failReason = context.HttpContext.Items["FailReason"];
        if (failReason is string s) throw NotAuthorised(s);
    
        // No token!
        throw NotAuthorised();
    }
}