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

    public async Task<IActionResult> Logout()
    {
        // Sign out from the cookie authentication scheme
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Sign out from Auth0 (if using OpenID Connect)
        await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
        {
            RedirectUri = Url.Action("SignIn", "Account", null, Request.Scheme) // Ensure full URL
        });

        // Explicitly redirect to SignIn after sign-out
        return RedirectToAction("SignIn", "Account");
    }

    public IActionResult SignIn()
    {
        return View();
    }
}
