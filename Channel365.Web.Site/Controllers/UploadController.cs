using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Channel365.Data.Entities;
using Channel365.Data.Layer;
using Channel365.Models;
using Channel365.Web.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Channel365.Web.Controllers {

    public class UploadController : Controller {
        private UnitOfWork _context;
        IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _appdbContext;

        public UploadController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<UploadController> logger,
            ApplicationDbContext dbContext
            ) {
            _appdbContext = dbContext;
            _dbContext = context;
            this._context = new UnitOfWork();
            _env = env;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult R() => RedirectToAction("Index", "Home");

        [Route("/account/upload")]
        public IActionResult Index() { return View(); }

        [HttpPost, ValidateAntiForgeryToken,
            RequestFormLimits(MultipartBodyLengthLimit = 268435456), RequestSizeLimit(268435456), Route("/account/upload")]
        public async Task<IActionResult> Index([Bind] VideoModel videoModel) {
            string files = Guid.NewGuid().ToString();
            string urlPage = "http://channel365.win19.se/" + files + "/";
            if (ModelState.IsValid && User.Identity.IsAuthenticated) {
                videoModel.Id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                videoModel.ApplicationUser = await _userManager.GetUserAsync(User);

                if (videoModel.File != null) {
                    var fileName = Path.GetFileName(videoModel.File.FileName);
                    if (System.IO.File.Exists(fileName))
                        System.IO.File.Delete(fileName);

                    var uploads = Path.Combine(_env.WebRootPath, files);
                    bool exists = Directory.Exists(uploads);
                    if (!exists)
                        Directory.CreateDirectory(uploads);

                    var ct = videoModel.File.ContentType;

                    var filePath = Path.Combine(uploads, fileName);
                    await videoModel.File.CopyToAsync(new FileStream(filePath, FileMode.Create));

                    videoModel.VideoPath = urlPage + fileName;
                    _appdbContext.Add(videoModel);
                    await _appdbContext.SaveChangesAsync();
                }
                return RedirectToAction(nameof(R));
            }
            return View(videoModel);
        }

        [Route("/edit/{id}")]
        public IActionResult Edit(Guid? id) {
            if (id == null) { return NotFound(); }

            var video = _context.VideoRepo.GetById(id);
            if (video == null)
                return NotFound();
            return View(video);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/edit/{id}")]
        public IActionResult Edit(Guid id, [Bind("VideoTitle,VideoDescription,MadeForKids,UrlSlug,VideoImage")] VideoModel videoModel) {
            if (id != videoModel.VideoId)
                return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.VideoRepo.Modify(videoModel);
                    _context.SaveChanges();
                } catch (DbUpdateConcurrencyException) {
                    if (!VideoExists(videoModel.VideoId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(videoModel);
        }

        [Route("/delete/{id}")]
        public IActionResult Delete(int? id) {
            if (id == null) {
                return NotFound();
            }
            var product = _context.VideoRepo.GetById(id);
            if (product == null) {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/delete/{id}")]
        public IActionResult DeleteConfirmed(Guid id) {
            var product = _context.VideoRepo.GetById(id);
            StorageService objBlob = new StorageService();
            objBlob.DeleteStorageData(product.VideoPath);
            _context.VideoRepo.Delete(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool VideoExists(System.Guid id) {
            return _context.VideoRepo.GetAll().Any(s => s.VideoId == id);
        }
    }
}