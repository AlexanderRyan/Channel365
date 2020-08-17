using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using Channel365.Data.Models.Video;
using Channel365.Models;
using Microsoft.AspNetCore.Identity;

namespace Channel365.Data.Entities {
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UrlSlug { get; set; }
        public Uri UserImageUrl { get; set; }
        public IList<LikeDislikeModel> LikesAndDislikes { get; set; } = new List<LikeDislikeModel>();

        public IList<UserFollow> Followings { get; set; } = new List<UserFollow>();
        public IList<UserFollow> Followers { get; set; } = new List<UserFollow>();

        [NotMapped]
        public int Likes => LikesAndDislikes.Where(x => x.Like == true).Count();
        [NotMapped]
        public int Dislikes => LikesAndDislikes.Where(x => x.Like == false).Count();

        [JsonIgnore]
        public IList<PlaylistModel> UserPlaylist { get; set; } = new List<PlaylistModel>();
        [JsonIgnore]
        public IList<VideoModel> VideoLibrary { get; set; } = new List<VideoModel>();
    }
}