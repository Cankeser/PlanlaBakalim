using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Models
{
    public class BusinessVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProfileImageUrl { get; set; }
        public string CategoryName { get; set; }     
        public string WorkingHours { get; set; }
        public string Address { get; set; }

        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }

        public bool IsOpenNow { get; set; }
        public bool IsFavorited { get; set; }
    }
}
