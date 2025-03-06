using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inventory.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace inventory.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

   public IActionResult Login(string returnUrl = "/")
    {
        var authenticationProperties = new AuthenticationProperties { RedirectUri = returnUrl };
        return Challenge(authenticationProperties, "Auth0");
    }

    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = Url.Action("SignIn", "Account")
        }, CookieAuthenticationDefaults.AuthenticationScheme, "Auth0");
    }

    public IActionResult SignIn()
    {
        return View();
    }
}
