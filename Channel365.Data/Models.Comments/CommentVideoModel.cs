using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Channel365.Data.Entities;

namespace Channel365.Models
{
    public class CommentVideoModel
    {
        [Key]
        public Guid CommentId { get; set; }
        public string CommentBody { get; set; }
        public DateTime PostedAt { get; set; }
        public string Name { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public Guid VideoId { get; set; }
        [JsonIgnore]
        public VideoModel Video { get; set; }

    }
}