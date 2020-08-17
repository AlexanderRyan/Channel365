using Channel365.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Channel365.Models
{
    public class PlaylistModel
    {
        [Key]
        public Guid PlaylistId { get; set; }

        public string PlaylistName { get; set; }

        public string PlaylistDesc { get; set; }

        public string UrlSlug { get; set; }

        public virtual IList<VideosInPlaylist> VideosInPlaylists { get; set; } = new List<VideosInPlaylist>();
        public virtual PlaylistVisibility PlaylistVisibility { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}