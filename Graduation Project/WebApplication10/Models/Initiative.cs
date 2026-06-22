using System.ComponentModel.DataAnnotations;

namespace PathwayPlatform.Models
{
    public class Initiative
    {
        public long Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
