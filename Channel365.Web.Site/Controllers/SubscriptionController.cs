using Channel365.Data.Entities;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Channel365.Web.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<SubscriptionController> logger;

        public SubscriptionController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SubscriptionController> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return LocalRedirect("/Identity/Account/Login");

            //user = await context.Users
            //    .Include(x => x.Followings).ThenInclude(x => x.Target)
            //    .SingleOrDefaultAsync(x => x.Id == user.Id);

            return View(user);
        }

        public async Task<IActionResult> Subscribed()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return LocalRedirect("/Identity/Account/Login");

            user = await context.Users
                .Include(x => x.Followings).ThenInclude(x => x.Target)
                .SingleOrDefaultAsync(x => x.Id == user.Id);

            return View(user);
        }

        public async Task<IActionResult> Subscribers()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return LocalRedirect("/Identity/Account/Login");

            var subscribers = await context.UserFollowings
                .Include(x => x.Observer)
                .Where(x => x.TargetId == user.Id)
                .ToListAsync();

            return View(subscribers);

        }
        public async Task<IActionResult> ChannelSubs(string id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null && user.Id == id)
                return RedirectToAction("Index");

            var channel = await context.Users
                .AsNoTracking()
                .Include(x => x.Followers).ThenInclude(x => x.Observer)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (channel == null)
                return NotFound();

            return View(channel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(string targetId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Json("404"); //! No logged in user

            if (targetId == user.Id)
                return Json("403"); //! Prevent user from subscribing to self

            var subscription = await context.UserFollowings
                .SingleOrDefaultAsync(x => x.TargetId == targetId && x.ObserverId == user.Id);

            //! User already subscribes to target, remove subscription
            if (subscription != null)
            {
                context.UserFollowings.Remove(subscription);
                await context.SaveChangesAsync();

                return Json("204");
            }

            //! Create new subscription
            subscription = new UserFollow
            {
                TargetId = targetId,
                ObserverId = user.Id,
                Follow = true
            };

            context.UserFollowings.Add(subscription);
            await context.SaveChangesAsync();

            return Json("201");
        }

        [HttpGet]
        public async Task<IActionResult> IsSubscribed(string channelId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Json("404");

            if (user.Id == channelId)
                return Json("self");

            var subscription = await context.UserFollowings
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.TargetId == channelId && x.ObserverId == user.Id);

            if (subscription == null)
                return Json("false");

            return Json("true");
        }
        [HttpGet]
        public async Task<IActionResult> SubscriberAmount(string channelId)
        {
            var channel = await context.Users
                .AsNoTracking()
                .Include(x => x.Followers)
                .SingleOrDefaultAsync(x => x.Id == channelId);

            var amount = channel.Followers.Count();

            return Json(amount);
        }
    }
}
