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
    [Authorize] // All actions in this controller require the user to be logged in.
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Booking/Index
        // This page lists all available cabins from the Cabins table.
        public async Task<IActionResult> Index()
        {
            var cabins = await _context.Cabins.ToListAsync();
            return View(cabins);
        }

        // GET: /Booking/Create/5
        // This page shows the booking form for a single, specific cabin.
        public async Task<IActionResult> Create(int id)
        {
            var cabin = await _context.Cabins.FindAsync(id);
            if (cabin == null)
            {
                return NotFound();
            }
            return View(cabin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int cabinId, DateTime bookingDate, int startTimeHour)
        {
            var cabin = await _context.Cabins.FindAsync(cabinId);
            var currentUser = await _userManager.GetUserAsync(User);

            if (cabin == null || currentUser == null)
            {
                return NotFound();
            }

            // 1. Calculate booking details
            var bookingStartTime = bookingDate.Date.AddHours(startTimeHour);
            var bookingEndTime = bookingStartTime.AddHours(1); // All bookings are 1 hour long
            var bookingCost = cabin.PricePerHour;

            // 2. Check user's credit balance
            if (currentUser.CreditBalance < bookingCost)
            {
                TempData["ErrorMessage"] = $"Insufficient funds. Your balance is {currentUser.CreditBalance:C}, but the booking costs {bookingCost:C}.";
                return RedirectToAction("Create", new { id = cabinId });
            }

            // 3. CRITICAL: Check for overlapping bookings for this specific cabin
            var isSlotTaken = await _context.Bookings
                .AnyAsync(b => b.CabinId == cabinId &&
                               bookingStartTime < b.EndTime &&
                               bookingEndTime > b.StartTime);

            if (isSlotTaken)
            {
                TempData["ErrorMessage"] = "This time slot is already booked. Please choose another time.";
                return RedirectToAction("Create", new { id = cabinId });
            }

            // 4. All checks passed, create the booking record
            var newBooking = new Booking
            {
                ApplicationUserId = currentUser.Id,
                CabinId = cabinId,
                StartTime = bookingStartTime,
                EndTime = bookingEndTime
            };

            // 5. Deduct credit and save the new booking
            currentUser.CreditBalance -= bookingCost;
            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Success! Your booking for {cabin.Location} on {bookingStartTime:f} is confirmed.";
            return RedirectToAction("MyBookings");
        }

        // GET: /Booking/MyBookings
        // This page shows the logged-in user's booking history.
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var userBookings = await _context.Bookings
                                       .Where(b => b.ApplicationUserId == userId)
                                       .Include(b => b.Cabin) // This is IMPORTANT: It joins the Cabin details to the booking.
                                       .OrderByDescending(b => b.StartTime)
                                       .ToListAsync();

            return View(userBookings);
        }
    }
}