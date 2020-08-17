using System;

namespace Channel365.Web.Models.Dto
{
    public class VideoDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string VideoImage { get; set; }
        public bool IsPrivate { get; set; }
    }
}
