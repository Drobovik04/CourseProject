using CourseProject;
using CourseProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IStringLocalizer<SharedResources> localizer)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _localizer = localizer;
    }

    // Login
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.UserNameOrEmail)
                   ?? await _userManager.FindByNameAsync(model.UserNameOrEmail);

        if (user == null)
        {
            ModelState.AddModelError("", _localizer["InvalidLoginAttempt"]);
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", _localizer["InvalidLoginAttempt"]);
        return View(model);
    }


    // Register
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);


        var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }


    // Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
