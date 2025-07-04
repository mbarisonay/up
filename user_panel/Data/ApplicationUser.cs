using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace user_panel.Data // Make sure this namespace matches your project name
{
    // This class extends the default IdentityUser with our custom fields
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CreditBalance { get; set; }
    }
}