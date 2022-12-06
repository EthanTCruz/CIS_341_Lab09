using Microsoft.AspNetCore.Mvc;
using Lab8.Data;
using Lab8.Models;
using Lab8.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Lab8.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Lab8.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CommunityStoreContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public CustomerController(CommunityStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public IActionResult SubmitListing()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<Listing> listings = await _context.Listings
                .Include(listing => listing.Condition)
                .Include(listing => listing.Store)
                .Include(listing => listing.ClaimedBy)
                .ToListAsync();

            foreach (Listing listing in listings)
            {

                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            List<ListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                string customerName = "Unclaimed";
                if (l.ClaimedByID != null)
                {
                    customerName = l.ClaimedBy.Name;
                }
                ListingDTO listingDTO = new()
                {
                    ListingID = l.ListingID,
                    Quantity = l.Quantity,
                    Description = l.Description,
                    CreatedBy = l.CreatedBy.Name,
                    ClaimedBy = customerName,
                    Store = l.Store.Name,
                    Condition = l.Condition.Description
                };
                listDTOs.Add(listingDTO);
            }

            return View(listDTOs);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Claim_Unclaim(int id)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

                    var user = await GetCurrentUserAsync();

                    var actual_customer = await _context.Customers
.FirstOrDefaultAsync(l => l.Email == user.Email);
                    var entity = await _context.Listings
                        .FirstOrDefaultAsync(l => l.ListingID == id);
                    if (entity.ClaimedByID is null)
                    {

                        entity.ClaimedBy = actual_customer;
                    }
                    else
                    {
                        if (entity.ClaimedBy == actual_customer)
                        {
                            entity.ClaimedBy = null;
                        }
                    }
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Listings.Any(e => e.ListingID == id))
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
            return RedirectToAction(nameof(Index));
        }


        [Authorize]
        public async Task<IActionResult> ViewSubmittedListings()
        {
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
            
            var user = await GetCurrentUserAsync();

            var userId = user?.Id;


            List<Listing> listings = await _context.Listings
                .Include(listing => listing.Condition)
                .Include(listing => listing.Store)
                .Where(Listing=>Listing.CreatedBy.Email==user.Email)
                .ToListAsync();

            //unecessary?
            foreach (Listing listing in listings)
            {

                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            List<ListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                if (l.ClaimedBy.Email.Equals(user.Email)) { 
                ListingDTO listingDTO = new()
                {
                    ListingID = l.ListingID,
                    Quantity = l.Quantity,
                    Description = l.Description,
                    CreatedBy = l.CreatedBy.Name,
                    ClaimedBy = l.ClaimedBy.Name,
                    Store = l.Store.Name,
                    Condition = l.Condition.Description
                };

                listDTOs.Add(listingDTO);
                }
            }

            return View(listDTOs);
        }

    }
}
