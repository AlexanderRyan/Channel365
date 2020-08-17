using Channel365.Data.Entities;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Channel365.Web.ViewComponents
{
    public class AddToPlaylistViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public AddToPlaylistViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user)
        {

            if (user != null)
            {
                var playlists = await context.Playlists
                .Include(x => x.VideosInPlaylists).ThenInclude(x => x.Video)
                .Include(x => x.ApplicationUser)
                .Where(x => x.ApplicationUser.Id == user.Id)
                .ToListAsync();

                return View(playlists);
            }
            
            return View();
        }
    }
}
