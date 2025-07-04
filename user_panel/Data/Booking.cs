using System;
using System.ComponentModel.DataAnnotations;

namespace user_panel.Data
{
    public class Booking
    {
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int CabinId { get; set; }
        public Cabin Cabin { get; set; }
    }
}