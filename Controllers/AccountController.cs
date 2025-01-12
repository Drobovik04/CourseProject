using CourseProject;
using CourseProject.Database;
using CourseProject.Models;
using CourseProject.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IStringLocalizer<SharedResources> localizer)
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


        var user = new AppUser { UserName = model.UserName, Email = model.Email };
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
    [Authorize]
    public async Task<IActionResult> Index(string sortColumnCreated = "Title", string sortOrderCreated = "asc", string sortColumnAnswered = "Title", string sortOrderAnswered = "asc", string viewModeCreated = "Table", string viewModeAnswered = "Table", string tab = "Created")
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var createdTemplates = _context.Templates
            .Include(x => x.Author)
            .Include(x => x.Topic)
            .Include(x => x.Comments)
            .Include(x => x.Likes)
            .Where(x => x.Author.Id == currentUser.Id)
            .AsQueryable();

        var answeredTemplates = _context.Forms
            .Where(x => x.UserId == currentUser.Id)
            .Include(x => x.Template)
            .Include(x => x.Template.Author)
            .Include(x => x.Template.Topic)
            .Include(x => x.Template.Comments)
            .Include(x => x.Template.Likes)
            .AsQueryable();

        createdTemplates = sortColumnCreated switch
        {
            "Title" => sortOrderCreated == "asc" ? createdTemplates.OrderBy(x => x.Title) : createdTemplates.OrderByDescending(x => x.Title),
            "Topic" => sortOrderCreated == "asc" ? createdTemplates.OrderBy(x => x.Topic.Name) : createdTemplates.OrderByDescending(x => x.Topic.Name),
            "Author" => sortOrderCreated == "asc" ? createdTemplates.OrderBy(x => x.Author.UserName) : createdTemplates.OrderByDescending(x => x.Author.UserName),
            "Comments" => sortOrderCreated == "asc" ? createdTemplates.OrderBy(x => x.Comments.Count) : createdTemplates.OrderByDescending(x => x.Comments.Count),
            "Likes" => sortOrderCreated == "asc" ? createdTemplates.OrderBy(x => x.Likes.Count) : createdTemplates.OrderByDescending(x => x.Likes.Count),
            "CreatedAt" => sortOrderCreated == "asc" ? createdTemplates.OrderBy(x => x.CreatedAt) : createdTemplates.OrderByDescending(x => x.CreatedAt),
            _ => createdTemplates
        };

        answeredTemplates = sortColumnAnswered switch
        {
            "Title" => sortOrderAnswered == "asc" ? answeredTemplates.OrderBy(x => x.Template.Title) : answeredTemplates.OrderByDescending(x => x.Template.Title),
            "Topic" => sortOrderAnswered == "asc" ? answeredTemplates.OrderBy(x => x.Template.Topic.Name) : answeredTemplates.OrderByDescending(x => x.Template.Topic.Name),
            "Author" => sortOrderAnswered == "asc" ? answeredTemplates.OrderBy(x => x.Template.Author.UserName) : answeredTemplates.OrderByDescending(x => x.Template.Author.UserName),
            "Comments" => sortOrderAnswered == "asc" ? answeredTemplates.OrderBy(x => x.Template.Comments.Count) : answeredTemplates.OrderByDescending(x => x.Template.Comments.Count),
            "Likes" => sortOrderAnswered == "asc" ? answeredTemplates.OrderBy(x => x.Template.Likes.Count) : answeredTemplates.OrderByDescending(x => x.Template.Likes.Count),
            "CreatedAt" => sortOrderAnswered == "asc" ? answeredTemplates.OrderBy(x => x.Template.CreatedAt) : answeredTemplates.OrderByDescending(x => x.Template.CreatedAt),
            "SubmittedAt" => sortOrderAnswered == "asc" ? answeredTemplates.OrderBy(x => x.SubmittedAt) : answeredTemplates.OrderByDescending(x => x.SubmittedAt),
            _ => answeredTemplates
        };

        var model = new ProfileViewModel
        {
            CreatedTemplates = await createdTemplates.ToListAsync(),
            AnsweredTemplates = await answeredTemplates.ToListAsync(),
            CurrentViewModeCreated = viewModeCreated,
            CurrentViewModeAnswered = viewModeAnswered,
            SortColumnCreated = sortColumnCreated,
            SortOrderCreated = sortOrderCreated,
            SortColumnAnswered = sortColumnAnswered,
            SortOrderAnswered = sortOrderAnswered,
            Tab = tab,
            UserId = currentUser.Id,
            HasAccountOnSalesforce = currentUser.SalesforceAccountId != null
        };

        return View(model);
    }
}
