using CourseProject;
using CourseProject.Database;
using CourseProject.ViewModels.Account;
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
            return RedirectToAction("Index", "Template");
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
            return RedirectToAction("Index", "Template");
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
        return RedirectToAction("Index", "Template");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var createdTemplates = await _context.Templates.Where(x => x.Author.Id == currentUser.Id).ToListAsync();

        var answeredTemplates = await _context.Forms.Where(fa => fa.UserId == currentUser.Id).Include(x => x.Template).ToListAsync();

        var model = new ProfileViewModel
        {
            CreatedTemplates = createdTemplates,
            AnsweredTemplates = answeredTemplates
        };

        return View(model);
    }
}
