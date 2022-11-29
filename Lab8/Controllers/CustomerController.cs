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
        public CustomerController(CommunityStoreContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SubmitListing()
        {
            return View();
        }





        [Authorize]
        public async Task<IActionResult> ViewListings()
        {
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
            
            var user = await GetCurrentUserAsync();

            var userId = user?.Id;


            List<Listing> listings = await _context.Listings
                .Include(listing => listing.Condition)
                .Include(listing => listing.Store)
                .ToListAsync();

            foreach (Listing listing in listings)
            {

                await _context.Entry(listing).Reference(l => l.Customer).LoadAsync();
            }

            List<ListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                ListingDTO listingDTO = new()
                {
                    ListingID = l.ListingID,
                    Quantity = l.Quantity,
                    Description = l.Description,
                    Customer = l.Customer.FirstName + " " + l.Customer.LastName,
                    Store = l.Store.Name,
                    Condition = l.Condition.Description
                };

                listDTOs.Add(listingDTO);
            }

            return View(listDTOs);
        }

    }
}
