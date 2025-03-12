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
            // Sign out from cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign out from Auth0 with a proper redirect to end session
            var callbackUrl = Url.Action("SignIn", "Account", null, Request.Scheme);
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
            {
                RedirectUri = callbackUrl + "?clearCache=true" // Force cache clear
            });

            // Clear any local session or cache
            HttpContext.Session.Clear(); // Ensure session is cleared if used
            Response.Cookies.Delete(".AspNetCore.Cookies"); // Explicitly delete cookie

            // Redirect to login page
            return Redirect(callbackUrl ?? "/");
        }

        // Action method to display the sign-in view
        public IActionResult SignIn()
        {
            return View();
        }
    }
}
