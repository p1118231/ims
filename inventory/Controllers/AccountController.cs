using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inventory.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace inventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        // Constructor to initialize the logger
        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        // Action method to handle login
        public IActionResult Login(string returnUrl = "/")
        {
            // Set the redirect URI after successful authentication
            var authenticationProperties = new AuthenticationProperties { RedirectUri = returnUrl };
            // Challenge the Auth0 authentication scheme
            return Challenge(authenticationProperties, "Auth0");
        }

        // Action method to handle logout
        public async Task<IActionResult> Logout()
        {
            // Sign out from the cookie authentication scheme
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign out from Auth0 (if using OpenID Connect)
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Redirect to SignIn action after sign-out
                RedirectUri = Url.Action("SignIn", "Account", null, Request.Scheme) // Ensure full URL
            });

            // Explicitly redirect to SignIn after sign-out
            return RedirectToAction("SignIn", "Account");
        }

        // Action method to display the sign-in view
        public IActionResult SignIn()
        {
            return View();
        }
    }
}
