using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Channel365.Data.Entities;
using Channel365.Data.Models.Video;
using Channel365.Models;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Channel365.Web.Controllers
{

    public class VideoController : Controller
    {
        private readonly ApplicationDbContext context;
        private UserManager<ApplicationUser> user;

        public VideoController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            this.context = context;
            this.user = user;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Video(Guid id)
        {
            var currentUser = await user.GetUserAsync(User);

            if (currentUser != null)
                ViewBag.User = currentUser;

            var video = await context.Videos
                .Include(x => x.ApplicationUser).ThenInclude(x => x.VideoLibrary)
                .Include(x => x.CommentVideos).ThenInclude(x => x.ApplicationUser)
                .Include(x => x.LikesAndDislikes)
                .FirstOrDefaultAsync(x => x.VideoId == id);

            if (video == null)
                return NotFound();

            return View(video);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(Guid videoId, string message)
        {
            //! Check if a user is logged in, if not, respond with 403 - Forbidden.
            var commenter = await user.GetUserAsync(User);
            if (commenter == null)
                return Json("403");

            var comments = new CommentVideoModel
            {
                VideoId = videoId,
                CommentBody = message,
                ApplicationUser = commenter,
                Name = User.Identity.Name,
                PostedAt = DateTime.Now
            };

            context.CommentVideos.Add(comments);
            await context.SaveChangesAsync();

            var response = await context.CommentVideos
                .AsNoTracking()
                .Include(x => x.ApplicationUser)
                .SingleOrDefaultAsync(x => x.CommentId == comments.CommentId);

            return Json(response);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(Guid videoId, Guid commentId)
        {
            var currentUser = await user.GetUserAsync(User);
            if (currentUser == null)
                return Json("404");

            var commentToDelete = await context.CommentVideos
                .SingleOrDefaultAsync(x => x.CommentId == commentId && x.VideoId == videoId && x.ApplicationUser.Id == currentUser.Id);

            if (commentToDelete == null)
                return Json("404");

            context.CommentVideos.Remove(commentToDelete);
            await context.SaveChangesAsync();

            return Json("204");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCommentCount(Guid videoId)
        {
            var obj = await context.CommentVideos
                .AsNoTracking()
                .Where(x => x.VideoId == videoId)
                .ToListAsync();

            var count = obj.Count();

            return Json(count);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVideo(Guid id)
        {
            // Get the logged in user
            var currentUser = await user.GetUserAsync(User);

            // If current user is null, something went wrong
            if (currentUser == null)
                return NotFound();

            // Get video from db and check if it's Id matches argument Id 
            // and that the videos related user-Id matches currentUser Id
            var video = await context.Videos
                        .FirstOrDefaultAsync(x => x.VideoId == id && x.ApplicationUser.Id == currentUser.Id);

            if (video == null)
                return NotFound();

            context.Videos.Remove(video);
            await context.SaveChangesAsync();

            return Redirect("/Identity/Account/Manage/Uploads/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeVideo(Guid videoId, bool like)
        {
            var usersId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usersId == null)
                return Json("403");

            var videoLikeStatus = await context.LikesAndDislikes
                .SingleOrDefaultAsync(x => x.VideoId == videoId && x.UserId == usersId);

            //! If no Like/Dislike exists, create new
            if (videoLikeStatus == null)
            {
                var newLikeStatus = new LikeDislikeModel
                {
                    UserId = usersId,
                    VideoId = videoId,
                    Like = like
                };
                context.LikesAndDislikes.Add(newLikeStatus);
                await context.SaveChangesAsync();

            } //! If new value != current, update current to new
            else if (videoLikeStatus.Like != like)
            {
                videoLikeStatus.Like = like;

                context.Entry(videoLikeStatus).State = EntityState.Modified;
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (context.LikesAndDislikes.Any(x => x.VideoId == videoId && x.UserId == usersId))
                        return NotFound();
                    else
                        throw;
                }
            }
            else
            {
                //! If new value == current, delete the like
                context.LikesAndDislikes.Remove(videoLikeStatus);
                await context.SaveChangesAsync();
            }

            //! Load video and store new nr of likes/dislikes
            var video = await context.Videos
                .AsNoTracking()
                .Include(x => x.LikesAndDislikes)
                .SingleOrDefaultAsync(x => x.VideoId == videoId);

            int likeCount = video.LikesAndDislikes
                .Where(x => x.Like == true)
                .Count();

            int dislikeCount = video.LikesAndDislikes
                .Where(x => x.Like == false)
                .Count();

            //! Format counts for easy split
            string result = likeCount + ":" + dislikeCount;

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLike(Guid videoId)
        {
            var usersId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var likeToRemove = await context.LikesAndDislikes.FirstOrDefaultAsync(x => x.VideoId == videoId && x.UserId == usersId);
            if (likeToRemove != null)
            {
                context.LikesAndDislikes.Remove(likeToRemove);
                await context.SaveChangesAsync();

                return Json("204");
            }

            return Json("404");
        }

        [HttpGet]
        public async Task<IActionResult> LikedVideos()
        {
            var currUser = await user.GetUserAsync(User);

            if (currUser == null)
                return LocalRedirect("/Identity/Account/Login");

            currUser = await context.Users
                .AsNoTracking()
                .Include(x => x.LikesAndDislikes).ThenInclude(x => x.Video).ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == currUser.Id);

            return View(currUser);
        }

        public IActionResult FollowUser(string userName)
        {
            var target = context.Users.FirstOrDefault(x => x.UserName == userName);
            var observer = User.FindFirstValue(ClaimTypes.NameIdentifier);



            return View();
        }

        public IActionResult Edit(Guid id)
        {
            var video = context.Videos.FirstOrDefault(x => x.VideoId == id);
            if (video == null)
            {
                return NotFound();
            }
            return View(video);
        }
        public IActionResult EditVideo(Guid VideoId, string VideoTitle, string VideoDescription, bool IsPrivate)
        {
            var editVideo = context.Videos.FirstOrDefault(x => x.VideoId == VideoId);

            editVideo.VideoTitle = VideoTitle ?? editVideo.VideoTitle;
            editVideo.VideoDescription = VideoDescription ?? editVideo.VideoDescription;
            editVideo.IsPrivate = IsPrivate;

            context.Entry(editVideo).State = EntityState.Modified;

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (context.Videos.Any(x => x.VideoId == VideoId))
                    return NotFound();
                else
                    throw;
            }
            return LocalRedirect("/Identity/Account/Manage/Uploads");
        }
    }
}