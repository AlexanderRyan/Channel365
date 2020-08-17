using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Channel365.Data.Entities;
using Channel365.Models;
using Channel365.Web.Data;
using Channel365.Web.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Channel365.Web.Areas.Identity.Pages.Account.Manage.Uploads
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public IList<VideoDTO> Uploads { get; set; } = new List<VideoDTO>();

        public IndexModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            Uploads = await context.Videos
                            .Include(x => x.ApplicationUser)
                            .Where(x => x.ApplicationUser.Id == user.Id)
                            .OrderByDescending(x => x.CreatedDate)
                            .Select(x => VideoToDto(x))
                            .ToListAsync();

            return Page();
        }
        private static VideoDTO VideoToDto(VideoModel video) =>
            new VideoDTO
            {
                Id = video.VideoId,
                Title = video.VideoTitle,
                Description = video.VideoDescription,
                CreatedDate = video.CreatedDate,
                VideoImage = video.VideoImage,
                IsPrivate = video.IsPrivate
            };
    }
}
