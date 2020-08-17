using Channel365.Data.Entities;
using Channel365.Models;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Channel365.Web.Controllers
{
    public class ChannelController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;

        public ChannelController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<IActionResult> Index(string id)
        {
            var user = await userManager.GetUserAsync(User);
            
            if (user != null && user.Id != id)
            {
                var subscription = await context.UserFollowings
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.ObserverId == user.Id && x.TargetId == id);

                if (subscription == null)
                    ViewData["Subscribed"] = false;
                else
                    ViewData["Subscribed"] = true;
            }

            var channel = await userManager.FindByIdAsync(id);
            if (channel == null)
                return NotFound();

            return View(channel);
        }
        public async Task<IActionResult> Videos(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            user.VideoLibrary = await context.Videos
                                .Where(x => x.ApplicationUser.Id == id && x.IsPrivate == false).ToListAsync();

            return View(user);
        }
        public async Task<IActionResult> Playlists(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            user.UserPlaylist = await context.Playlists
                                .Where(x => x.ApplicationUser.Id == id && x.PlaylistVisibility == PlaylistVisibility.Public)
                                .Include(x => x.VideosInPlaylists).ThenInclude(x => x.Video)
                                .ToListAsync();

            return View(user);
        }
        public async Task<IActionResult> Subscriptions(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // Load subscribers from user
            //var user.Following = await context.User.Include(x => x.Subscribers)ToListAsync();

            return View(user);
        }
    }
}