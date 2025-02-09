﻿using System;
using Microsoft.AspNetCore.Mvc;

namespace LhDev.DemoOne.Attributes;

/// <summary>
/// Use on a <see cref="ControllerBase"/> to require that controller or method be protected by JWT bearer authorisation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiJwtAuthoriseAttribute : JwtAuthoriseAttribute
{
    protected override string Type => "bearer";
}