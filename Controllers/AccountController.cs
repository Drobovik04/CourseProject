using CourseProject;
using CourseProject.Database;
using CourseProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public AccountController(AppDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IStringLocalizer<SharedResources> localizer)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _localizer = localizer;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.UserNameOrEmail) ?? await _userManager.FindByNameAsync(model.UserNameOrEmail);

        if (user == null)
        {
            ModelState.AddModelError("", _localizer["InvalidLoginAttempt"]);
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Form");
        }

        ModelState.AddModelError("", _localizer["InvalidLoginAttempt"]);
        return View(model);
    }


    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);


        var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Form");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var createdForms = await _context.Forms.Where(f => f.Author.Id == currentUser.Id).ToListAsync();

        var answeredForms = await _context.FormAnswers.Where(fa => fa.UserId == currentUser.Id).Include(fa => fa.Form).ToListAsync();

        var model = new ProfileViewModel
        {
            CreatedForms = createdForms,
            AnsweredForms = answeredForms
        };

        return View(model);
    }
}
