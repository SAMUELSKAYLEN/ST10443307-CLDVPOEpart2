using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10443307CLVDProjectPOEPart2.Models;

namespace ST10443307CLVDProjectPOEPart2.Controllers
{
    public class BookingController : Controller

    {
        private readonly EventEaseContext _context;

        public BookingController(EventEaseContext context)
        {
            _context = context;
        }
        //list of bookings
        public async Task<IActionResult> Index(string searchString)
        {
            var Bookings = _context.Bookings
                .Include(i => i.Venue)
                .Include(i => i.Event)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                Bookings = Bookings.Where(i =>
                    i.Venue.VenueName.Contains(searchString) ||
                    i.Event.EventName.Contains(searchString));
            }

            return View(await Bookings.ToListAsync());
        }

        public IActionResult Create()
        {   
            ViewBag.Venues = _context.Venues.ToList();
            ViewBag.Events = _context.Events.ToList();
            return View();
        }

        // Create Booking (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            var selectedEvent = await _context.Events.FirstOrDefaultAsync(e => e.EVENTID == booking.EVENTID);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewBag["Venues"] = _context.Venues.ToList();
                ViewBag["Events"] = _context.Events.ToList();
                return View(booking);
            }

            var conflict = await _context.Bookings
                .Include(b => b.Event)
                .AnyAsync(b => b.VENUEID == booking.VENUEID &&
                               b.Event.EventDate.Date == selectedEvent.EventDate.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");

                ViewBag.Venues = _context.Venues.ToList();
                ViewBag.Events = _context.Events.ToList();
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully. ";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // If database constraint fails (e.g., unique key violation), show friendly message
                    ModelState.AddModelError("", "This venue is already booked for that date.");
                    ViewBag["Events"] = _context.Events.ToList();
                    ViewBag["Venues"] = _context.Venues.ToList();
                    return View(booking);
                }
            }

            ViewBag.Venues = _context.Venues.ToList(); // Re-populate dropdowns if validation fails
            ViewBag.Events = _context.Events.ToList();
            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            //var Booking = await _context.Bookings.FirstOrDefaultAsync(m => m.BOOKINGID == id);

            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include (b => b.Event) // Include Event data
                .Include(b => b.Venue) // Include Venue data
                .FirstOrDefaultAsync(m => m.BOOKINGID == id);

            if (booking == null)
            {
                return NotFound();
            }           

            return View(booking);
        }

        //delete booking
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) 
            { 
                return NotFound();
            }
            var Booking = await _context.Bookings
                .Include(b => b.Event)  
                .Include(b => b.Venue)  
                .FirstOrDefaultAsync(m => m.BOOKINGID == id);


            if (Booking == null)
            {
                return NotFound();
            }

            return View(Booking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var Booking = await _context.Bookings.FindAsync(id);
            _ = _context.Bookings.Remove(Booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(m => m.BOOKINGID == id);
        }

        //edit booking
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Booking = await _context.Bookings.FindAsync(id);

            if (Booking == null)
            {
                return NotFound();
            }

            // Populate dropdowns for Events and Venues
            ViewBag.Venues = new SelectList(await _context.Venues.ToListAsync(), 
                             "VENUEID", "VenueName", Booking.VENUEID);
            ViewBag.Events = new SelectList(await _context.Events.ToListAsync(), "EVENTID"
                            ,"EventName", Booking.EVENTID);

            return View(Booking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking Booking)
        {
            if (id != Booking.BOOKINGID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {

                try
                {
                    _context.Update(Booking);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(Booking.EVENTID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(await _context.Venues.ToListAsync(),
                            "VENUEID", "VenueName", Booking.VENUEID);
            ViewBag.Events = new SelectList(await _context.Events.ToListAsync(),
                            "EVENTID", "EventName", Booking.EVENTID);

            return View(Booking);
        }
        

    }
}

