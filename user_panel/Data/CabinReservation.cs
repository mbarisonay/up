using System;
using System.ComponentModel.DataAnnotations;

namespace user_panel.Data
{
    // This class represents a single reservation in your "Cabins" table.
    public class CabinReservation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; } // e.g., "Bornova/İzmir"

        [Required]
        public string Description { get; set; }

        // --- These columns replace 'Booked' to handle time slots ---
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        // --- Link to the user who made the reservation ---
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}