using System.Linq;
using System.Threading.Tasks;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Channel365.Web.ViewComponents {
    public class VideoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext context;

        public VideoViewComponent(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var videos = await context.Videos
                .AsNoTracking()
                .Include(x => x.ApplicationUser)
                .Include(x => x.VideosInPlaylists)
                .Include(x => x.CommentVideos)
                .Include(x => x.LikesAndDislikes)
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.IsPrivate == false)
                .ToListAsync();

            return View(videos);
        }
    }
}