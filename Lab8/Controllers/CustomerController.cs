using Microsoft.AspNetCore.Mvc;
using Lab8.Data;
using Lab8.Models;
using Lab8.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Lab8.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Formats.Asn1.AsnWriter;

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

        public ListingDTO ConvertToListingDTO(Listing listing)
        {
            return new ListingDTO
            {
                ListingID = listing.ListingID,
                Condition = listing.Condition.Description,
                Description = listing.Description,
                Quantity = listing.Quantity,
                CreatedBy = listing.CreatedBy.Name,
                ClaimedBy = listing.ClaimedBy?.Name ?? "Unclaimed",
                Store = listing.Store.Name,
                Status = listing.Status.Description,
                Type = listing.Type.Name,
                TypeName = listing.Type.Name,
                TypeDescription = listing.Type.Description
            };
        }

        public Listing ConvertToListing(ListingDTO listingDTO)
        {
            return new Listing
            {
                ListingID = listingDTO.ListingID,
                Description = listingDTO.Description,
                Quantity = listingDTO.Quantity,
                Condition = new Condition { Description = listingDTO.Condition },
                Type = new Models.Type { Name = listingDTO.Type, Description = listingDTO.TypeDescription },
                Status = new Status { Description = listingDTO.Status },
                Store = new Store { Name = listingDTO.Store },
                CreatedBy = new Customer { Name = listingDTO.CreatedBy },
                ClaimedBy = listingDTO.ClaimedBy == "Unclaimed" ? null : new Customer { Name = listingDTO.ClaimedBy }
            };
        }


        // GET: ListingDTOes/Delete/5
        public async Task<IActionResult> DeleteListing(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

            var user = await GetCurrentUserAsync();

            var actual_customer = await _context.Customers
.FirstOrDefaultAsync(l => l.Email == user.Email);
            var listing = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .Where(l => l.CreatedBy.CustomerID == actual_customer.CustomerID)
                .FirstOrDefaultAsync(m => m.ListingID == id);

            if (listing == null)
            {
                return NotFound();
            }
            var listingDTO = ConvertToListingDTO(listing);
            return View(listingDTO);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ListingID)
        {
            if (_context.Listings == null)
            {
                return Problem("Entity set 'CommunityStoreContext.ListingDTO'  is null.");
            }


            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

            var user = await GetCurrentUserAsync();

            var actual_customer = await _context.Customers
.FirstOrDefaultAsync(l => l.Email == user.Email);
            var listingDTO = await _context.Listings
                .Where(l => l.CreatedBy.Email == actual_customer.Email)
                .FirstOrDefaultAsync(l => ListingID == l.ListingID);
            if (listingDTO != null)
            {
                _context.Listings.Remove(listingDTO);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> CreateListing()
        {
            // Retrieve the list of unique stores from the database
            var stores = await _context.Stores.Select(s => s.Name).Distinct().ToListAsync();

            // Convert the list of stores to a list of SelectListItem objects
            var storeItems = stores.Select(s => new SelectListItem { Value = s, Text = s }).ToList();

            // Create the view model and set the Stores property to the list of SelectListItem objects
            var viewModel = new ListingDTO { Stores = storeItems };

            return View(viewModel);
        }





        // POST: ListingDTOes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListingID,Condition,Description,Quantity,Store,TypeDescription,TypeName")] ListingDTO listingDTO)
        {


            if (ModelState.IsValid)
            {
                var status = new Status
                {
                    Description = "Unapproved"
                };
                var type = new Models.Type
                {
                    Name = listingDTO.TypeName,
                    Description = listingDTO.TypeDescription
                };
                var condition = new Condition
                {
                    Description = listingDTO.Condition
                };
                _context.Status.Add(status);
                _context.Conditions.Add(condition);
                _context.Types.Add(type);
                await _context.SaveChangesAsync();

                Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
                var user = await GetCurrentUserAsync();

                var actual_customer = await _context.Customers
                    .FirstOrDefaultAsync(l => l.Email == user.Email);
                var store = await _context.Stores
                    .FirstOrDefaultAsync(l => l.Name == listingDTO.Store);

                var listing = new Listing
                {
                    Description = listingDTO.Description,
                    Quantity = listingDTO.Quantity,
                    CreatedBy = actual_customer,
                    ClaimedBy = null,
                    Store = store,
                    Condition = condition,
                    Type = type,
                    Status = status
                };

                _context.Listings.Add(listing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SubmittedListings));
            }
            return RedirectToAction(nameof(SubmittedListings));
        }


        [Authorize]
        public async Task<IActionResult> SubmittedListings()
        {
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
            
            var user = await GetCurrentUserAsync();

            var userId = user?.Id;


            List<Listing> listings = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
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
                string customerName = "Unclaimed";
                if (l.ClaimedByID != null)
                {
                    customerName = "ClaimedListings";
                }
                if (l.CreatedBy.Email.Equals(user.Email))
                    {
                        ListingDTO listingDTO = new()
                        {
                            ListingID = l.ListingID,
                            Quantity = l.Quantity,
                            Description = l.Description,
                            CreatedBy = l.CreatedBy.Name,
                            ClaimedBy = customerName,
                            Store = l.Store.Name,
                            Condition = l.Condition.Description,
                            TypeDescription = l.Type.Description,
                            TypeName = l.Type.Name
                        };

                        listDTOs.Add(listingDTO);
                    
                }
            }

            return View(listDTOs);
        }

        [Authorize]
        public async Task<IActionResult> ClaimedListings()
        {
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

            var user = await GetCurrentUserAsync();

            var userId = user?.Id;


            List<Listing> listings = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .Where(Listing => Listing.ClaimedBy.Email == user.Email)
                .ToListAsync();

            //unecessary?
            foreach (Listing listing in listings)
            {

                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            List<ListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                if (l.ClaimedBy != null)
                {
                    if (l.ClaimedBy.Email.Equals(user.Email))
                    {
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
            }

            return View(listDTOs);
        }

    }
}
