using CourseProject.Database;
using CourseProject.Hubs;
using CourseProject.Models;
using CourseProject.ViewModels.Comment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<CommentHub> _hubContext;

        public CommentController(AppDbContext context, UserManager<AppUser> userManager, IHubContext<CommentHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> GetComments(int templateId)
        {
            var comments = await _context.Comments
                .Where(x => x.TemplateId == templateId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.Text,
                    x.CreatedAt,
                    Author = x.User.UserName
                })
                .ToListAsync();

            return Ok(comments);
        }
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentAddViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                Text = model.Text,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUser.Id,
                TemplateId = model.TemplateId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(model.TemplateId.ToString()).SendAsync("ReceiveComment", new
            {
                comment.Id,
                comment.UserId,
                comment.Text,
                comment.CreatedAt,
                Author = currentUser.UserName
            });

            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateViewModel model)
        {
            var comment = await _context.Comments.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (!isAdmin && currentUser != comment.User)
            {
                return Forbid();
            }

            if (comment == null)
            {
                return NotFound();
            }

            comment.Text = model.Text;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(comment.TemplateId.ToString()).SendAsync("UpdateComment", new
            {
                comment.Id,
                comment.Text
            });

            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (!isAdmin && currentUser != comment.User)
            {
                return Forbid();
            }

            if (comment == null)
            {
                return NotFound();
            }

            var templateId = comment.TemplateId;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(templateId.ToString()).SendAsync("DeleteComment", comment.Id);

            return Ok();
        }
    }
}
