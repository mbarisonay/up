using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_panel.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace user_panel.Controllers
{
    [Authorize] // All actions still require a user to be logged in
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context; // Our connection to the database
        private readonly UserManager<ApplicationUser> _userManager;

        // The hard-coded list of cabins has been DELETED from here.
        // We no longer need it.

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Booking/Index
        // This action now reads from your 'Cabins' table in Azure.
        public async Task<IActionResult> Index()
        {
            var cabinsFromDb = await _context.Cabins.ToListAsync();
            return View(cabinsFromDb);
        }

        // GET: /Booking/Create/5
        // This action now finds a cabin by its unique ID from the database.
        public async Task<IActionResult> Create(int id)
        {
            var cabin = await _context.Cabins.FindAsync(id);
            if (cabin == null)
            {
                return NotFound(); // Safety check
            }
            return View(cabin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int cabinId, DateTime bookingDate, int startTimeHour)
        {
            // First, get the cabin details (like price) from the database using its ID
            var cabin = await _context.Cabins.FindAsync(cabinId);
            var currentUser = await _userManager.GetUserAsync(User);

            if (cabin == null || currentUser == null)
            {
                return NotFound();
            }

            // 1. Calculate booking details
            var bookingStartTime = bookingDate.Date.AddHours(startTimeHour);
            var bookingEndTime = bookingStartTime.AddHours(1);
            var bookingCost = cabin.PricePerHour; // Use the price from the database

            // 2. Check user's credit balance (same logic as before)
            if (currentUser.CreditBalance < bookingCost)
            {
                TempData["ErrorMessage"] = $"Insufficient funds. Your balance is {currentUser.CreditBalance:C}, but the booking costs {bookingCost:C}.";
                return RedirectToAction("Create", new { id = cabinId });
            }

            // 3. CRITICAL: Check for overlapping bookings for this specific cabinId
            var isSlotTaken = await _context.CabinReservations
                .AnyAsync(r => r.Location == cabin.Location && 
                               bookingStartTime < r.EndTime &&
                               bookingEndTime > r.StartTime);

            if (isSlotTaken)
            {
                TempData["ErrorMessage"] = "This time slot is already booked for this location. Please choose another time.";
                return RedirectToAction("Create", new { id = cabinId });
            }

            // 4. All checks passed, create the reservation record
            var newReservation = new CabinReservation
            {
                ApplicationUserId = currentUser.Id,
                Location = cabin.Location,
                Description = cabin.Description,
                StartTime = bookingStartTime,
                EndTime = bookingEndTime
            };

            // 5. Deduct credit and save everything (same logic as before)
            currentUser.CreditBalance -= bookingCost;
            _context.CabinReservations.Add(newReservation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Success! Your reservation for {cabin.Location} on {bookingStartTime:f} is confirmed.";
            return RedirectToAction("MyBookings");
        }

        // This action does not need any changes. It already works with the database.
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var userBookings = await _context.CabinReservations
                                       .Where(b => b.ApplicationUserId == userId)
                                       .OrderByDescending(b => b.StartTime)
                                       .ToListAsync();

            return View(userBookings);
        }
    }
}