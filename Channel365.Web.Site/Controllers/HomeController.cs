using System.Diagnostics;
using System.Linq;
using Channel365.Web.Data;
using Channel365.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Channel365.Web.Controllers {
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            this.context = context;
            
        }

        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var videos = await context.Videos
                .AsNoTracking()
                .Include(x => x.LikesAndDislikes)
                .OrderByDescending(x => x.LikesAndDislikes.Count())
                .Take(5)
                .ToListAsync();
            
            return View(videos);         
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}