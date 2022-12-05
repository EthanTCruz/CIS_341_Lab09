using Lab8.Areas.Identity.Data;
using Lab8.Data;
using Lab8.Models;
using Lab8.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lab8.Controllers
{

    public class HomeController : Controller
    {
        private readonly CommunityStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(CommunityStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;


        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<Listing> listings = await _context.Listings
                .Include(listing => listing.Condition)
                .Include(listing => listing.Store)
                .ToListAsync();

            foreach (Listing listing in listings)
            {

                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            List<ListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                string customerName = "Unclaimed" ;
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
                        ClaimedBy = l.ClaimedBy.Name,
                        Store = l.Store.Name,
                        Condition = l.Condition.Description
                    };
                listDTOs.Add(listingDTO);
            }

            return View(listDTOs);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Claim_Unclaim(int id, [Bind("ListingID")] ListingDTO listing)
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
                    } else {
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
                return Redirect("/Home/Index");
            }
            return Redirect("/Home/Index");
        }

    
}
}