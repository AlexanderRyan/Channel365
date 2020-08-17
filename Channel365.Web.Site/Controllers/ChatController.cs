using System;
using System.Threading.Tasks;
using Channel365.Models;
using Channel365.Web.Data;
using Channel365.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Channel365.Web.Controllers {
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private IHubContext<ChatHub> chat;
        public ChatController(IHubContext<ChatHub> chat, ApplicationDbContext context)
        {
            this.chat = chat;
            this.context = context;
        }

        public ApplicationDbContext context { get; set; }

        [HttpPost("[action]/{connectionId}/{videoName}")]

        public async Task<IActionResult> WatchVideo(string videoName, string connectionId)
        {
            await chat.Groups.AddToGroupAsync(connectionId, videoName);
            return Ok();
        }
        [HttpPost("[action]/{connectionId}/{videoName}")]
        public async Task<IActionResult> LeaveVideoPage(string videoName, string connectionId)
        {
            await chat.Groups.RemoveFromGroupAsync(connectionId, videoName);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string message, string videoName, Guid videoId)
        {

            var messages = new CommentVideoModel
            {
                VideoId = videoId,
                CommentBody = message,
                Name = User.Identity.Name,
                PostedAt = DateTime.Now

            };

            context.CommentVideos.Add(messages);

            await context.SaveChangesAsync();

            await chat.Clients.Group(videoName)
                .SendAsync("RecieveMessage", messages);
            return Ok();
        }
    }
}