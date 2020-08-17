using System;
using Channel365.Data.Entities;
using Channel365.Models;

namespace Channel365.Data.Models.Video {
    public class LikeDislikeModel
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid VideoId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public VideoModel Video { get; set; }
        public bool Like { get; set; }
    }
}
