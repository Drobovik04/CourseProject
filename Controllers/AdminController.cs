using CourseProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CourseProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IStringLocalizer<SharedResources> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index(string sortColumn, string sortOrder)
        {
            var users = _userManager.Users.ToList(); 
            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsAdmin = roles.Contains("Admin"),
                    IsLockedOut = user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow
                });
            }


            ViewData["SortOrder"] = sortOrder == "asc" ? "desc" : "asc";
            ViewData["SortColumn"] = sortColumn;

            model = (sortColumn switch
            {
                "UserName" => sortOrder == "asc" ? model.OrderBy(u => u.UserName) : model.OrderByDescending(u => u.UserName),
                "Email" => sortOrder == "asc" ? model.OrderBy(u => u.Email) : model.OrderByDescending(u => u.Email),
                "IsAdmin" => sortOrder == "asc" ? model.OrderBy(u => u.IsAdmin) : model.OrderByDescending(u => u.IsAdmin),
                "IsLockedOut" => sortOrder == "asc" ? model.OrderBy(u => u.IsLockedOut) : model.OrderByDescending(u => u.IsLockedOut),
                _ => model.OrderBy(u => u.UserId),
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, bool isAdmin)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (isAdmin)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(100));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", _localizer["ErrorDelete"]) ;
            return RedirectToAction("Index");
        }
    }
}
