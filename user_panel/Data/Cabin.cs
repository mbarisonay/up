using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // This line must be here

namespace user_panel.Data
{
    [Table("cab")] // This tells it to use the name "cab"
    public class Cabin
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerHour { get; set; }
    }
}