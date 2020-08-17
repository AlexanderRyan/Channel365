using Channel365.Data.Entities;
using Channel365.Models;
using System.Collections.Generic;

namespace Channel365.Web.Models
{
    public class SearchViewModel
    {
        public IList<VideoModel> Videos { get; set; } = new List<VideoModel>();
        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public IList<PlaylistModel> Playlists { get; set; } = new List<PlaylistModel>();

        public string SearchString { get; set; }
        public string SearchFilter { get; set; }
    }
}
