using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Channel365.Data.Entities;
using Channel365.Models;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Channel365.Web.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<IndexModel> logger;


        public IList<VideoModel> Videos { get; set; } = new List<VideoModel>();
        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public IList<PlaylistModel> Playlists { get; set; } = new List<PlaylistModel>();

        public IndexModel(
            ApplicationDbContext context,
            ILogger<IndexModel> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Videos = await context.Videos
                .AsNoTracking()
                .Take(5)
                .ToListAsync();

            Users = await context.Users
                .AsNoTracking()
                .Take(5)
                .ToListAsync();

            Playlists = await context.Playlists
                .AsNoTracking()
                .Include(x => x.VideosInPlaylists).ThenInclude(x => x.Video)
                .Take(5)
                .ToListAsync();

            return Page();
        }
    }
}
