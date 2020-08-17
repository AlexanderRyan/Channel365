using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Channel365.Data.Entities;
using Channel365.Models;
using Channel365.Web.Data;
using Channel365.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Channel365.Web.Controllers
{
    public class SearchController : Controller
    {

        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public SearchController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; } = "videos";


        public async Task<IActionResult> Index()
        {
            var searchViewModel = await loadOnFilterAsync();

            return View(searchViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateFilter()
        {
            var newModel = await loadOnFilterAsync();

            return Json(newModel);
        }


        /// <summary>
        /// Instantiates DTO with data from query depending on filter.
        /// </summary>
        /// <returns></returns>
        private async Task<SearchViewModel> loadOnFilterAsync()
        {
            var model = new SearchViewModel
            {
                SearchString = SearchQuery,
                SearchFilter = Filter
            };

            if (!string.IsNullOrEmpty(Filter))
            {

                switch (Filter.ToLower())
                {
                    case "videos":

                        if (!string.IsNullOrEmpty(SearchQuery))
                        {
                            model.Videos = await context.Videos
                                    .AsNoTracking()
                                    .Where(x => x.VideoTitle.Contains(SearchQuery) && x.IsPrivate == false)
                                    .Include(x => x.ApplicationUser)
                                    .Include(x => x.LikesAndDislikes)
                                    .Include(x => x.CommentVideos)
                                    .ToListAsync();
                        }
                        else
                        {
                            model.Videos = await context.Videos
                                .AsNoTracking()
                                .Where(x => x.IsPrivate == false)
                                .Include(x => x.ApplicationUser)
                                .Include(x => x.LikesAndDislikes)
                                .Include(x => x.CommentVideos)
                                .ToListAsync();
                        }

                        break;

                    case "users":

                        if (!string.IsNullOrEmpty(SearchQuery))
                        {
                            model.Users = await context.Users
                                        .AsNoTracking()
                                        .Include(x => x.Followers)
                                        .Where(x => x.UserName.Contains(SearchQuery))
                                        .ToListAsync();
                        }
                        else
                        {
                            model.Users = await context.Users
                                .AsNoTracking()
                                .Include(x => x.Followers)
                                .ToListAsync();
                        }
                        break;

                    case "playlists":

                        if (!string.IsNullOrEmpty(SearchQuery))
                        {
                            model.Playlists = await context.Playlists
                                            .AsNoTracking()
                                            .Where(x => x.PlaylistName.Contains(SearchQuery) && x.PlaylistVisibility == PlaylistVisibility.Public)
                                            .Include(x => x.VideosInPlaylists)
                                            .Include(x => x.ApplicationUser)
                                            .ToListAsync();
                        }
                        else
                        {
                            model.Playlists = await context.Playlists
                                .AsNoTracking()
                                .Where(x => x.PlaylistVisibility == PlaylistVisibility.Public)
                                .Include(x => x.VideosInPlaylists)
                                .Include(x => x.ApplicationUser)
                                .ToListAsync();
                        }
                        break;
                }
            }

            return model;
        }

        // Check if user is logged in
        [HttpGet]
        public async Task<IActionResult> GetLoggedInState()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Json(false);

            return Json(true);
        }
    }
}
