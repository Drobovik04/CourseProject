using CourseProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Страница админ панели с пользователями
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList(); // Получаем всех пользователей
            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsAdmin = roles.Contains("Admin"),
                    IsLockedOut = user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow // Проверка на блокировку
                });
            }

            return View(model);
        }

        // Обновление роли пользователя (назначить/снять роль Admin)
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
                // Назначаем роль Admin
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                // Снимаем роль Admin
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            return RedirectToAction("Index");
        }

        // Блокировка пользователя
        [HttpPost]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(100)); // Блокировка на очень долгий срок
            return RedirectToAction("Index");
        }

        // Разблокировка пользователя
        [HttpPost]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null); // Разблокировка
            return RedirectToAction("Index");
        }

        // Удаление пользователя
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user); // Удаляем пользователя
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error deleting user");
            return RedirectToAction("Index");
        }
    }
}
