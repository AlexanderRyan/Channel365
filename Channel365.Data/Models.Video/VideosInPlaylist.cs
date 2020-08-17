using System;
using System.Text.Json.Serialization;

namespace Channel365.Models
{
    public class VideosInPlaylist
    {
        public Guid PlaylistId { get; set; }
        [JsonIgnore]
        public PlaylistModel Playlist { get; set; }
        public Guid VideoId { get; set; }
        public VideoModel Video { get; set; }
    }
}