using System.ComponentModel.DataAnnotations;

namespace ST10443307CLVDProjectPOEPart2.Models
{
    public class Event
    {
        public int EVENTID { get; set; }

        [Required]

        public required string EventName { get; set; }

        [Required]

        public DateTime EventDate { get; set; }
        public string? Descript { get; set; }
        public int VENUEID { get; set; }
        


        public Venue? Venue { get; set; } //Each Event has one Venue
        public List<Booking> Bookings { get; set; } = new();
    }
}
