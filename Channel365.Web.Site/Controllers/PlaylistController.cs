using Channel365.Data.Entities;
using Channel365.Models;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Channel365.Web.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public PlaylistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index(string msg)
        {
            ViewData["PlaylistStatus"] = msg;

            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return LocalRedirect("/Identity/Account/Login");

            user = await context.Users
                        .AsNoTracking()
                        .Include(x => x.UserPlaylist).ThenInclude(x => x.VideosInPlaylists)
                        .FirstOrDefaultAsync(x => x.Id == user.Id);

            if (user == null)
                return NotFound("Error: Unable to locate specified resource.");

            return View(user);
        }

        // Retrieves a specific playlists
        public async Task<IActionResult> Playlist(Guid id)
        {
            ViewData["IsOwner"] = false;
            var currentUser = await userManager.GetUserAsync(User);

            var playlist = await context.Playlists
                .AsNoTracking()
                .Include(x => x.ApplicationUser)
                .Include(x => x.VideosInPlaylists).ThenInclude(x => x.Video).ThenInclude(x => x.ApplicationUser)
                .Include(x => x.VideosInPlaylists).ThenInclude(x => x.Video).ThenInclude(x => x.LikesAndDislikes)
                .SingleOrDefaultAsync(x => x.PlaylistId == id);

            if (playlist == null)
                return NotFound("Error: specifed resource could not be located.");

            if (currentUser != null && playlist.ApplicationUser.Id == currentUser.Id)
            {
                ViewData["IsOwner"] = true;
                return View(playlist);
            }

            else if (playlist.PlaylistVisibility == PlaylistVisibility.Public)
                return View(playlist);

            else
                return View();

        }

        // Retrieves all playlists from specific user/channel
        public async Task<IActionResult> Playlists(string id)
        {
            var currentUser = await userManager.GetUserAsync(User);

            if (currentUser != null && currentUser.Id == id)
                return RedirectToAction("Index");

            var user = await context.Users
                .AsNoTracking()
                .Include(x => x.UserPlaylist).ThenInclude(x => x.VideosInPlaylists)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound("Error: specified resource could not be located.");

            return View(user);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string id, string name, string desc, int visibility)
        {
            var userId = userManager.GetUserId(User);

            if (userId != id)
                return Json("403");

            if (name != null)
            {
                var newPlaylist = new PlaylistModel
                {
                    ApplicationUser = await userManager.GetUserAsync(User),
                    PlaylistId = new System.Guid(),
                    PlaylistName = name,
                    PlaylistDesc = desc ?? "",
                    PlaylistVisibility = (PlaylistVisibility)visibility,
                    UrlSlug = name.Replace(' ', '-').ToLower()
                };

                context.Playlists.Add(newPlaylist);
                await context.SaveChangesAsync();

                var createdPlaylist = await context.Playlists
                    .AsNoTracking()
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.VideosInPlaylists)
                    .SingleOrDefaultAsync(x => x.PlaylistId == newPlaylist.PlaylistId);

                return Json(createdPlaylist);
            }

            return Json("404");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = userManager.GetUserId(User);

            var playlist = await context.Playlists
                .FirstOrDefaultAsync(x => x.PlaylistId == id && x.ApplicationUser.Id == userId);

            if (playlist == null)
                return Json("404");

            context.Playlists.Remove(playlist);
            await context.SaveChangesAsync();

            return Json("204");
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylistAmount()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return Json("404");

            user = await context.Users
                .AsNoTracking()
                .Include(x => x.UserPlaylist)
                .SingleOrDefaultAsync(x => x.Id == user.Id);

            var amount = user.UserPlaylist.Count().ToString();

            return Json(amount);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylistVideoAmount(Guid id)
        {
            var playlist = await context.Playlists
                .AsNoTracking()
                .Include(x => x.VideosInPlaylists)
                .FirstOrDefaultAsync(x => x.PlaylistId == id);

            if (playlist == null)
                return Json("404");

            var amount = playlist.VideosInPlaylists.Count();

            return Json(amount);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, string newTitle, string newDesc, string newVis)
        {

            //todo Make sure newVis != null, refactor $.post in Index.cshtml

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Json("404");

            var playlistToEdit = await context.Playlists.SingleOrDefaultAsync(x => x.PlaylistId == id);
            if (playlistToEdit == null)
                return Json("404");

            playlistToEdit.PlaylistName = newTitle ?? playlistToEdit.PlaylistName;
            playlistToEdit.PlaylistDesc = newDesc ?? playlistToEdit.PlaylistDesc;
            playlistToEdit.PlaylistVisibility = (PlaylistVisibility)int.Parse(newVis);

            context.Entry(playlistToEdit).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (context.Playlists.Any(x => x.PlaylistId == id))
                {
                    return Json("409");
                }
                else
                    throw;
            }

            return Json("204");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToPlaylist(Guid playlistId, Guid videoId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Json("404");

            var playlist = await context.Playlists
                .Include(x => x.VideosInPlaylists).ThenInclude(x => x.Video)
                .FirstOrDefaultAsync(x => x.PlaylistId == playlistId && x.ApplicationUser.Id == user.Id);
            if (playlist == null)
                return Json("404");

            var video = await context.Videos
                .FirstOrDefaultAsync(x => x.VideoId == videoId);
            if (video == null)
                return Json("404");

            bool alreadyAdded = false;
            if (playlist.VideosInPlaylists.Count() > 0)
            {
                foreach (var item in playlist.VideosInPlaylists)
                {
                    if (item.Video.VideoId == video.VideoId)
                        alreadyAdded = true;
                }
            }

            if (alreadyAdded)
            {
                // status code "SEE OTHER" - indicates resource already exists
                return Json("303");
            }

            var addVideo = new VideosInPlaylist
            {
                PlaylistId = playlistId,
                VideoId = videoId
            };

            context.VideosInPlaylist.Add(addVideo);
            await context.SaveChangesAsync();

            return Json("201");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromPlaylist(Guid playlistId, Guid videoId) 
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Json("404");

            var playlist = await context.Playlists.SingleOrDefaultAsync(x => x.PlaylistId == playlistId);
            if (playlist == null)
                return Json("404");

            var video = await context.Videos.SingleOrDefaultAsync(x => x.VideoId == videoId);
            if (video == null)
                return Json("404");

            var videoPlaylistLink = await context.VideosInPlaylist
                .SingleOrDefaultAsync(x => x.VideoId == videoId && x.PlaylistId == playlistId);

            context.VideosInPlaylist.Remove(videoPlaylistLink);
            await context.SaveChangesAsync();

            return Json("204");

        }
    }
}
