using CourseProject.Database;
using CourseProject.Hubs;
using CourseProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Controllers
{
    public class LikeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<LikeHub> _hubContext;

        public LikeController(AppDbContext context, UserManager<AppUser> userManager, IHubContext<LikeHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int templateId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var template = await _context.Templates.Include(x => x.Likes).FirstOrDefaultAsync(x => x.Id == templateId);

            if (template == null)
            {
                return NotFound();
            }

            var existingLike = template.Likes.FirstOrDefault(x => x.User == currentUser);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
                await _hubContext.Clients.Group(templateId.ToString()).SendAsync("DeleteLike");
            }
            else
            {
                var like = new Like
                {
                    TemplateId = templateId,
                    UserId = currentUser.Id
                };

                _context.Likes.Add(like);
                await _hubContext.Clients.Group(templateId.ToString()).SendAsync("ReceiveLike");
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                likesCount = template.Likes.Count,
                userHasLiked = existingLike == null
            });
        }
    }
}
