namespace Channel365.Data.Entities
{
    public class UserFollow
    {
        public string TargetId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ApplicationUser Target { get; set; }
        public string ObserverId { get; set; }
        public ApplicationUser Observer { get; set; }
        public bool Follow { get; set; }

    }
}
