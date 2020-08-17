using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Channel365.Models;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Channel365.Web.Areas.Admin.Pages.Playlists
{
    public class ManageModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<ManageModel> logger;

        public IList<PlaylistModel> Playlists { get; set; } = new List<PlaylistModel>();

        public ManageModel(
            ApplicationDbContext context,
            ILogger<ManageModel> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Playlists = await context.Playlists
                .AsNoTracking()
                .Include(x => x.VideosInPlaylists)
                .Include(x => x.ApplicationUser)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetFindMatchAsync(string search)
        {
            IList<PlaylistModel> playlists = new List<PlaylistModel>();


            if (search != null && search.IndexOf(':') == 1)
            {
                var result = search.Split(':');

                switch (result[0])
                {
                    case "@":
                        playlists = await context.Playlists
                                .AsNoTracking()
                                .Include(x => x.ApplicationUser)
                                .Include(x => x.VideosInPlaylists)
                                .Where(x => x.ApplicationUser.UserName.Contains(result[1]))
                                .ToListAsync();
                        break;

                    case "?":
                        playlists = await context.Playlists
                            .AsNoTracking()
                            .Where(x => x.PlaylistId == Guid.Parse(result[1]))
                            .ToListAsync();
                        break;
                }
            }
            else
            {
                if (search == null)
                {
                    playlists = await context.Playlists
                        .AsNoTracking()
                        .Include(x => x.VideosInPlaylists)
                        .Include(x => x.ApplicationUser)
                        .ToListAsync();

                    return new JsonResult(playlists);
                }

                playlists = await context.Playlists
                        .AsNoTracking()
                        .Where(x => x.PlaylistName.Contains(search))
                        .Include(x => x.VideosInPlaylists)
                        .Include(x => x.ApplicationUser)
                        .ToListAsync();
            }
            

            return new JsonResult(playlists);
        }
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var playlist = await context.Playlists.FirstOrDefaultAsync(x => x.PlaylistId == id);

            string statusMessage;

            if (playlist == null)
            {
                statusMessage = "Error: Specified resource could not be located. (Playlist Id: )" + id;
                logger.LogInformation($"Playlist with ID {id} could not be found.");
                return new JsonResult(statusMessage);
            }

            context.Playlists.Remove(playlist);
            await context.SaveChangesAsync();

            statusMessage = id + " successfully removed.";
            logger.LogInformation($"Playlist with ID {id} has been removed from the database.");
            return new JsonResult(statusMessage);
        }
        public async Task<IActionResult> OnPostEditAsync(Guid id, string playlistName, string playlistDesc, int visibility)
        {
            var playlistToEdit = await context.Playlists.FirstOrDefaultAsync(x => x.PlaylistId == id);
            string statusMessage;

            if (playlistToEdit == null)
            {
                statusMessage = $"Error: unable to locate specified resource. Playlist id: {id}";
                logger.LogWarning($"Error locating Ppaylist. (id: {id})");

                return new JsonResult(statusMessage);
            }

            playlistToEdit.PlaylistName = playlistName ?? playlistToEdit.PlaylistName;
            playlistToEdit.PlaylistDesc = playlistDesc ?? playlistToEdit.PlaylistDesc;
            playlistToEdit.PlaylistVisibility = (PlaylistVisibility)visibility;

            context.Entry(playlistToEdit).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (context.Playlists.Any(x => x.PlaylistId == id))
                {
                    statusMessage = $"Concurrency Exception - Playlists id ({id}).";
                    logger.LogError($"Concurrency Exception - Playlists id ({id}).");
                    return new JsonResult("");
                }
                else
                    throw;
            }

            statusMessage = $"{id} successfully edited.";
            logger.LogInformation($"Playlist updated. (id: {id})");
            return new JsonResult(statusMessage);
        }
        public async Task<IActionResult> OnGetReturnEditedAsync(Guid id)
        {
            var playlist = await context.Playlists
                .AsNoTracking()
                .Include(x => x.ApplicationUser)
                .Include(x => x.VideosInPlaylists)
                .FirstOrDefaultAsync(x => x.PlaylistId == id);

            if (playlist == null)
            {
                logger.LogError($"Error retrieving element from database: {id}");
                return NotFound("Error: Unable to located specified resource.");
            }

            return new JsonResult(playlist);
        }

    }
}
