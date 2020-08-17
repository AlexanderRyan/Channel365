using Channel365.Models;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Channel365.Web.Views.Home
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private ApplicationDbContext context;

        public IList<VideoModel> VideoList { get; set; } = new List<VideoModel>();
      
        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            this.logger = logger;
            this.context = context;
        }

        public void OnGet()
        {
            VideoList = context.Videos.ToList();
        }
    }
}