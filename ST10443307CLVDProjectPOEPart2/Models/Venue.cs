using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ST10443307CLVDProjectPOEPart2.Models
{
    public class Venue
    {
        public int VENUEID { get; set; }

        [Required]

        public string? VenueName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]

        public int VenueCapacity { get; set; }
        //store URL images
        public string? VenueLocation { get; set; }

        [Required]
 
        public string? ImageURL { get; set; }
        //upload from create or edit form
        [NotMapped]

        public IFormFile? ImageFile { get; set; }

        public List<Booking> Bookings { get; set; } = new();
    }
}
