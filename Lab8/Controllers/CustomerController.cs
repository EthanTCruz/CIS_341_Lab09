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
using System.Reflection;

namespace Lab8.Controllers
{
    [Authorize(Roles = "Customer")]
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

        /// <summary>
        /// Displays a confirmation page for deleting a listing.
        /// </summary>
        /// <param name="id">The ID of the listing to be deleted.</param>
        /// <returns>The delete confirmation page for the listing.</returns>
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteListing(int? id)
        {
            // Return NotFound if id is null or _context.Listings is null
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            // Get the current user
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
            var user = await GetCurrentUserAsync();

            // Find the actual customer with the matching email
            var actual_customer = await _context.Customers
                .FirstOrDefaultAsync(l => l.Email == user.Email);

            // Find the listing associated with the customer
            var listing = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .Where(l => l.CreatedBy.CustomerID == actual_customer.CustomerID)
                .FirstOrDefaultAsync(m => m.ListingID == id);

            // Return NotFound if listing is null
            if (listing == null)
            {
                return NotFound();
            }

            // Convert the listing to a ListingDTO
            var listingDTO = ConvertToListingDTO(listing);
            return View(listingDTO);
        }

        /// <summary>
        /// Deletes a listing from the database.
        /// </summary>
        /// <param name="ListingID">The ID of the listing to be deleted.</param>
        /// <returns>The index page for the customer's listings, or a problem page if the delete operation fails.</returns>
        [Authorize(Roles = "Customer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ListingID)
        {
            // Return Problem if _context.Listings is null
            if (_context.Listings == null)
            {
                return Problem("Entity set 'CommunityStoreContext.ListingDTO' is null.");
            }

            // Get the current user
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
            var user = await GetCurrentUserAsync();

            // Find the actual customer with the matching email
            var actual_customer = await _context.Customers
                .FirstOrDefaultAsync(l => l.Email == user.Email);

            // Find the listing associated with the customer
            var listingDTO = await _context.Listings
                .Where(l => l.CreatedBy.Email == actual_customer.Email)
                .FirstOrDefaultAsync(l => ListingID == l.ListingID);

            // Remove the listing if it exists
            if (listingDTO != null)
            {
                _context.Listings.Remove(listingDTO);
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Displays the details of a specific listing.
        /// </summary>
        /// <param name="id">The ID of the listing to be displayed.</param>
        /// <returns>The details page for the listing, or a not found page if the listing is not found.</returns>
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            // Return NotFound if id is null or _context.Listings is null
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            // Find the listing
            var listing = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .FirstOrDefaultAsync(m => m.ListingID == id);

            // Return NotFound if listing is null
            if (listing == null)
            {
                return NotFound();
            }

            // Convert the listing to a ListingDTO
            var listingDTO = ConvertToListingDTO(listing);
            return View(listingDTO);
        }


        /// <summary>
        /// Displays a list of the customer's listings.
        /// </summary>
        /// <returns>The index page for the customer's listings.</returns>
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {
            // Find all approved or received listings
            List<Listing> listings = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .Where(l => l.Status.Description == "Approved" || l.Status.Description == "Recieved" && (l.ClaimedBy == null || l.ClaimedBy.Name != "Unclaimed"))
                .ToListAsync();

            // Load the createdBy references for each listing
            foreach (Listing listing in listings)
            {
                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            // Initialize a list of ListingDTOs
            List<ListingDTO> listDTOs = new();

            // Convert each listing to a ListingDTO and add it to the list
            foreach (Listing l in listings)
            {
                // Set the customerName to "Unclaimed" if the listing is not claimed, otherwise set it to the name of the claiming customer
                string customerName = "Unclaimed";
                if (l.ClaimedByID != null)
                {
                    customerName = l.ClaimedBy.Name;
                }

                // Convert the listing to a ListingDTO
                ListingDTO listingDTO = ConvertToListingDTO(l);
                listDTOs.Add(listingDTO);
            }

            return View(listDTOs);
        }


        /// <summary>
        /// Claims or unclaims a listing.
        /// </summary>
        /// <param name="id">The ID of the listing to be claimed or unclaimed.</param>
        /// <returns>The details page for the listing, or a not found page if the listing is not found.</returns>
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> Claim_Unclaim(int id)
        {
            // Return if the model state is invalid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current user
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
                    var user = await GetCurrentUserAsync();

                    // Find the customer with the same email as the current user
                    var actual_customer = await _context.Customers
                        .FirstOrDefaultAsync(l => l.Email == user.Email);

                    // Find the listing with the specified id
                    var entity = await _context.Listings
                        .FirstOrDefaultAsync(l => l.ListingID == id);

                    // If the listing is not claimed, claim it for the current customer
                    if (entity.ClaimedByID is null)
                    {
                        entity.ClaimedBy = actual_customer;
                    }
                    // If the listing is already claimed by the current customer, unclaim it
                    else
                    {
                        if (entity.ClaimedBy == actual_customer)
                        {
                            entity.ClaimedBy = null;
                        }
                    }

                    // Save the changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Return NotFound if the listing does not exist
                    if (!_context.Listings.Any(e => e.ListingID == id))
                    {
                        return NotFound();
                    }
                    // Otherwise, throw the exception
                    else
                    {
                        throw;
                    }

                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }


        
        /// <summary>
        /// Displays the create listing form.
        /// </summary>
        /// <returns>The create listing form page.</returns>
        [Authorize(Roles = "Customer")]
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





        /// <summary>
        /// Creates a new listing.
        /// </summary>
        /// <param name="listing">The listing to be created.</param>
        /// <returns>The listings index page if the listing is successfully created, or the create listing form page if the creation fails.</returns>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ListingID,Condition,Description,Quantity,Store,TypeDescription,TypeName")]
    ListingDTO listingDTO)
        {
            if (ModelState.IsValid)
            {
                // Create a new status, type, and condition for the listing
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

                // Add the new status, type, and condition to the database and save the changes
                _context.Status.Add(status);
                _context.Conditions.Add(condition);
                _context.Types.Add(type);
                await _context.SaveChangesAsync();

                // Get the current user
                Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
                var user = await GetCurrentUserAsync();

                // Get the customer associated with the current user
                var actual_customer = await _context.Customers
                    .FirstOrDefaultAsync(l => l.Email == user.Email);

                // Get the store associated with the listing
                var store = await _context.Stores
                    .FirstOrDefaultAsync(l => l.Name == listingDTO.Store);

                // Create a new listing with the provided information
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

                // Add the new listing to the database and save the changes
                _context.Listings.Add(listing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SubmittedListings));
            }
            return RedirectToAction(nameof(SubmittedListings));
        }


        /// <summary>
        /// Displays the submitted listings for the current user.
        /// </summary>
        /// <returns>The submitted listings page.</returns>
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SubmittedListings()
        {
            // helper function to get the current user
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

            // get the current user
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;

            // get the listings that are created by the current user
            List<Listing> listings = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .Where(Listing => Listing.CreatedBy.Email == user.Email)
                .ToListAsync();

            // iterate through the listings and load the reference for the created by field
            // this may not be necessary as it's already included in the query above
            foreach (Listing listing in listings)
            {
                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            // create a list of listing DTOs
            List<ListingDTO> listDTOs = new();

            // iterate through the listings and convert them to DTOs
            // only add the DTO to the list if it was created by the current user
            foreach (Listing l in listings)
            {
                if (l.CreatedBy.Email.Equals(user.Email))
                {
                    ListingDTO listingDTO = ConvertToListingDTO(l);
                    listDTOs.Add(listingDTO);
                }
            }

            return View(listDTOs);
        }



        /// <summary>
        /// Displays the listings claimed by the current user.
        /// </summary>
        /// <returns>The claimed listings page.</returns>
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ClaimedListings()
        {
            // helper function to get the current user
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

            // get the current user
            var user = await GetCurrentUserAsync();

            // get the listings that are claimed by the current user
            List<Listing> listings = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .Where(Listing => Listing.ClaimedBy.Email == user.Email)
                .ToListAsync();

            // iterate through the listings and load the reference for the created by field
            // this may not be necessary as it's already included in the query above
            foreach (Listing listing in listings)
            {
                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            // create a list of listing DTOs
            List<ListingDTO> listDTOs = new();

            // iterate through the listings and convert them to DTOs
            // only add the DTO to the list if it was claimed by the current user
            foreach (Listing l in listings)
            {
                if (l.ClaimedBy != null)
                {
                    if (l.ClaimedBy.Email.Equals(user.Email))
                    {
                        ListingDTO listingDTO = ConvertToListingDTO(l);
                        listDTOs.Add(listingDTO);
                    }
                }
            }

            return View(listDTOs);
        }


    }
}
