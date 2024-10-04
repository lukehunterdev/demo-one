using System;
using LhDev.DemoOne.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LhDev.DemoOne.Services;
using Microsoft.AspNetCore.Http;

namespace LhDev.DemoOne.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController(ILogger<HomeController> logger, IDbUserService dbUserService, IJwtService jwtService) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() 
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });


    [HttpGet]
    public IActionResult Login() => View();


    [HttpPost]
    public async Task<IActionResult> Login(LoginUser loginUser)
    {
        // If user is logged in, then just redirect.
        var cookie = Request.Cookies[Program.CookieSession];
        if (!string.IsNullOrEmpty(cookie)) return RedirectToAction("List", "CustomersWeb");

        var user = await dbUserService.AuthenticateUserAsync(loginUser.Username, loginUser.Password);

        if (user == null) throw new Exception("Not authorised");

        var expires = DateTime.Now.AddMinutes(15);
        var token = jwtService.GenerateToken(user, expires);

        Response.Cookies.Append(Program.CookieSession, token, new CookieOptions { Expires = expires });

        return RedirectToAction("List", "CustomersWeb");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        var cookie = Request.Cookies[Program.CookieSession];

        if (!string.IsNullOrEmpty(cookie)) Response.Cookies.Delete(Program.CookieSession);

        return Redirect("/");
    }

    [HttpGet]
    public IActionResult HealthCheck() => View();
}