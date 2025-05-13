using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using ST10443307CLVDProjectPOEPart2.Models;
using static System.Net.Mime.MediaTypeNames;
using Azure.Storage.Blobs;


namespace ST10443307CLVDProjectPOEPart2.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseContext _context;

        public VenueController(EventEaseContext context) 
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Venue = await _context.Venues.ToListAsync();
            return View(Venue);
        }

        //Confirm deletion
        public async Task<IActionResult> Delete(int? id)
        {
            var Venue = await _context.Venues.FirstOrDefaultAsync(n => n.VENUEID == id);

            if (Venue == null)
            {
                return NotFound();
            }
            return View(Venue);
        }

        //Perform deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Delete(int id)
        {
            var Venue = await _context.Venues.FindAsync(id);
            if (Venue == null) return NotFound();

            var hasBookings = await _context.Bookings.AnyAsync(b => b.VENUEID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete venue because it has existing bookings";
                return RedirectToAction(nameof(Index));
            }

            _ = _context.Venues.Remove(Venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var Venue = await _context.Venues.FirstOrDefaultAsync(m => m.VENUEID == id);

            if (Venue == null)
            {
                return NotFound();
            }
            return View(Venue);
        }

        public IActionResult Create() 
        { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue) {
            if (ModelState.IsValid)
            { 
                if(venue.ImageFile != null ) 
                {
                    // upload image to azure blob storage
                var bloburl = await UploadImageToBlobAsync(venue.ImageFile);
                    //save blob storage url to imageUrl property (database)
                venue.ImageURL = bloburl;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully. ";
                return RedirectToAction(nameof(Index));
            }        
            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Venue = await _context.Venues.FindAsync(id);

            if (Venue == null)
            {
                return NotFound();
            }
            return View(Venue);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VENUEID)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        // upload image to azure blob storage
                        var bloburl = await UploadImageToBlobAsync(venue.ImageFile);
                        //update venue.ImageUrl with new blob url
                        venue.ImageURL = bloburl;
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully. ";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VENUEID))

                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
        // This will upload the Image to Blob Storage Account
        // Uploads an image to Azure Blob Storage and returns the Blob URL

        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "";
            var containerName = "cldvpoecontainer";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(m => m.VENUEID == id);
        }
    }
}
