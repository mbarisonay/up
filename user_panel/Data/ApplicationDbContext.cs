using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace user_panel.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cabin> Cabins { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        // This method will run when the database is first created.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // This code adds the initial list of cabins to the Cabins table.
            builder.Entity<Cabin>().HasData(
                new Cabin { Id = 1, Location = "Bornova/İzmir", Description = "A modern, compact cabin perfect for a high-intensity workout. Equipped with a smart treadmill and weight set.", PricePerHour = 25.00m },
                new Cabin { Id = 2, Location = "Karşıyaka/İzmir", Description = "Spacious cabin with a focus on yoga and flexibility. Includes a full-length mirror and yoga mats.", PricePerHour = 25.00m },
                new Cabin { Id = 3, Location = "Çankaya/Ankara", Description = "Premium cabin featuring a rowing machine and advanced monitoring systems for performance tracking.", PricePerHour = 30.00m },
                new Cabin { Id = 4, Location = "Akyurt/Ankara", Description = "Standard cabin with essential cardio and strength training equipment. Great for a balanced workout.", PricePerHour = 20.00m },
                new Cabin { Id = 5, Location = "Beşiktaş/İstanbul", Description = "An urban-style cabin with a boxing bag and a high-performance stationary bike.", PricePerHour = 35.00m },
                new Cabin { Id = 6, Location = "Fatih/İstanbul", Description = "Historic district cabin offering a quiet and serene environment for mindful exercise and meditation.", PricePerHour = 30.00m }
            );
        }
    }
}