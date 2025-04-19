using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EpinHell.Models;

namespace EpinHell.Controllers
{
    public class AccountController : Controller
    {
        // Dependency injection for UserManager, SignInManager, and RoleManager
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructor to initialize the managers
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

            // Ensure that roles "Admin" and "User" are created (This should ideally be moved to a service or startup configuration to avoid side effects in the constructor)
            roleManager.CreateAsync(new IdentityRole("Admin"));
            roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Display the registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handle user registration form submissions
        [HttpPost]
        [ValidateAntiForgeryToken] // Protect against Cross-Site Request Forgery (CSRF)
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validate the model data
            if (!ModelState.IsValid)
                return View(model);

            // Create a new IdentityUser instance with the provided details
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };

            // Attempt to create the user in the database
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Sign in the newly created user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Add any errors to the ModelState for display in the view
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        // Display the login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handle user login form submissions
        [HttpPost]
        [ValidateAntiForgeryToken] // Protect against CSRF
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validate the model data
            if (!ModelState.IsValid)
                return View(model);

            // Attempt to sign in the user with the provided credentials
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Add an error message to the ModelState for invalid login attempts
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(model);
        }

        // Handle user logout
        [HttpPost]
        [ValidateAntiForgeryToken] // Protect against CSRF
        public async Task<IActionResult> Logout()
        {
            // Sign out the current user
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
