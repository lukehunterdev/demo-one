using System;
using LhDev.DemoOne.Attributes;
using LhDev.DemoOne.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LhDev.DemoOne.Controllers;

[ApiController]
[ApiException]
[Route("api/test")]
public class TestApiController
{
    /// <summary>
    /// Throws a general exception to test exception handling middleware.
    /// </summary>
    /// <returns>Never returns.</returns>
    /// <exception cref="Exception">This is a general exception.</exception>
    [HttpGet("generalException")]
    public IActionResult ThrowGeneralException()
    {
        throw new Exception("This is a general exception.");
    }


    /// <summary>
    /// Throws a not implemented exception to test exception handling middleware.
    /// </summary>
    /// <returns>Never returns.</returns>
    /// <exception cref="NotImplementedException">This is a not implemented exception.</exception>
    [HttpGet("notImplementedException")]
    public IActionResult ThrowNotImplementedException()
    {
        throw new NotImplementedException();
    }



    /// <summary>
    /// Throws a Demo One exception to test exception handling middleware.
    /// </summary>
    /// <returns>Never returns.</returns>
    /// <exception cref="DemoOneWebException">This is a Demo One exception.</exception>
    [HttpGet("demoOneException")]
    public IActionResult ThrowDemoOneException()
    {
        throw DemoOneWebException.TestException();
    }
}