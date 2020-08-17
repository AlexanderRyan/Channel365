using Channel365.Models;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Channel365.Web.Areas.Admin.Pages.Videos
{
    public class ManageModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<ManageModel> logger;

        public IList<VideoModel> Videos { get; set; } = new List<VideoModel>();

        public ManageModel(
            ApplicationDbContext context,
            ILogger<ManageModel> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Videos = await context.Videos
                .AsNoTracking()
                .Include(x => x.ApplicationUser)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetFindMatchAsync(string videoTitle)
        {
            IList<VideoModel> videos = new List<VideoModel>();

            if (videoTitle != null && videoTitle.IndexOf(':') == 1)
            {
                var result = videoTitle.Split(':');

                switch (result[0])
                {
                    case "@":
                        videos = await context.Videos
                                .AsNoTracking()
                                .Include(x => x.ApplicationUser)
                                .Where(x => x.ApplicationUser.UserName.Contains(result[1]))
                                .ToListAsync();
                        break;

                    case "?":
                        videos = await context.Videos
                            .AsNoTracking()
                            .Include(x => x.ApplicationUser)
                            .Where(x => x.Id == result[1])
                            .ToListAsync();
                        break;
                }
            }
            else
            {
                if (videoTitle == null)
                {
                    videos = await context.Videos
                        .AsNoTracking()
                        .Include(x => x.ApplicationUser)
                        .OrderBy(x => x.CreatedDate)
                        .ToListAsync();

                    return new JsonResult(videos);
                }

                videos = await context.Videos
                    .AsNoTracking()
                    .Where(x => x.VideoTitle.Contains(videoTitle))
                    .Include(x => x.ApplicationUser)
                    .OrderBy(x => x.CreatedDate)
                    .ToListAsync();
            }

            return new JsonResult(videos);
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var video = await context.Videos.FirstOrDefaultAsync(x => x.VideoId == id);
            string statusMessage;

            if (video == null)
            {
                statusMessage = "Error: Specified resource could not be located. (Video Id: )" + id;
                return new JsonResult(statusMessage);
            }

            context.Videos.Remove(video);
            await context.SaveChangesAsync();

            statusMessage = id + " successfully removed.";

            return new JsonResult(statusMessage);
        }

        public async Task<IActionResult> OnPostEditAsync(Guid id, string title, string desc, bool isPrivate)
        {
            var videoToEdit = await context.Videos.FirstOrDefaultAsync(x => x.VideoId == id);
            string statusMessage;

            if (videoToEdit == null)
            {
                statusMessage = $"Error: unable to locate specified resource. Video id: {id}";
                logger.LogWarning($"Error locating video resource. (id: {id})");

                return new JsonResult(statusMessage);
            }

            videoToEdit.VideoTitle = title ?? videoToEdit.VideoTitle;
            videoToEdit.VideoDescription = desc ?? videoToEdit.VideoDescription;
            videoToEdit.IsPrivate = isPrivate;
            videoToEdit.UrlSlug = videoToEdit.VideoTitle.Replace(' ', '-').ToLower();
            videoToEdit.UpdatedDate = DateTime.Now;

            context.Entry(videoToEdit).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (context.Videos.Any(x => x.VideoId == id))
                {
                    statusMessage = $"Concurrency Exception - Video id ({id}).";
                    logger.LogError($"Concurrency Exception - Video id ({id}).");
                    return new JsonResult("");
                }
                else
                    throw;
            }

            statusMessage = $"{id} successfully edited.";
            logger.LogInformation($"Video updated. (id: {id})");
            return new JsonResult(statusMessage);
        }
        public async Task<IActionResult> OnGetReturnEditedAsync(Guid id)
        {
            var video = await context.Videos
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.VideoId == id);

            if (video == null)
            {
                logger.LogError($"Error retrieving element from database: {id}");
                return NotFound("Error: Unable to located specified resource.");
            }

            return new JsonResult(video);
        }
    }
}
