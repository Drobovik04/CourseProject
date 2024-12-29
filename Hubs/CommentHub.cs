using Microsoft.AspNetCore.SignalR;

namespace CourseProject.Hubs
{
    public class CommentHub : Hub
    {
        public async Task JoinTemplateGroup(int templateId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, templateId.ToString());
        }

        public async Task LeaveTemplateGroup(int templateId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, templateId.ToString());
        }
    }
}
