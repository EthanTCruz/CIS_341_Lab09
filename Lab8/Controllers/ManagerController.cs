using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab8.Data;
using Lab8.Models.DTO;
using Lab8.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Lab8.Models;

namespace Lab8.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly CommunityStoreContext _context;

        private readonly UserManager<ApplicationUser> _userManager;


        public ManagerController(CommunityStoreContext context, UserManager<ApplicationUser> userManager)
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
        /// Displays the details for the specified listing.
        /// </summary>
        /// <param name="id">The ID of the listing to display.</param>
        /// <returns>The listing details page.</returns>
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            // Return NotFound if the id is null or the Listings collection is null
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            // Load the Listing entity from the database and include related entities
            var listing = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .FirstOrDefaultAsync(m => m.ListingID == id);

            // Return NotFound if the listing is null
            if (listing == null)
            {
                return NotFound();
            }

            // Convert the Listing entity to a ListingDTO object
            ListingDTO listingDTO = ConvertToListingDTO(listing);

            return View(listingDTO);
        }





        /// <summary>
        /// Displays the unapproved listings for the current user's store.
        /// </summary>
        /// <returns>The unapproved listings page.</returns>
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UnapprovedListings()
        {
            // Get the current user
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
            var user = await GetCurrentUserAsync();

            // If the user is null, try to get the current user again
            if (user == null)
            {
                user = await GetCurrentUserAsync();
            }

            // Load unapproved listings for the current user's store from the database and include related entities
            List<Listing> listings = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .Where(l => l.Status.Description == "Unapproved" && l.Store.Manager.Email == user.Email)
                .ToListAsync();

            // Load the CreatedBy reference for each listing
            foreach (Listing listing in listings)
            {
                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            // Convert the listings to ListingDTO objects and add them to a list
            List<ListingDTO> listDTOs = new List<ListingDTO>();
            foreach (Listing l in listings)
            {
                // Only add listings with the "Unapproved" status to the list
                if (l.Status.Description == "Unapproved")
                {
                    ListingDTO listingDTO = ConvertToListingDTO(l);
                    listDTOs.Add(listingDTO);
                }
            }

            // Return the list of ListingDTO objects to the view
            return View(listDTOs);
        }



        /// <summary>
        /// Displays the approved listings for the current user's store.
        /// </summary>
        /// <returns>The approved listings page.</returns>
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApprovedListings()
        {
            // Get the current user
            Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
            var user = await GetCurrentUserAsync();

            // Load all listings for the current user's store from the database and include related entities
            List<Listing> listings = await _context.Listings
                .Include(l => l.ClaimedBy)
                .Include(l => l.Condition)
                .Include(l => l.CreatedBy)
                .Include(l => l.Status)
                .Include(l => l.Store)
                .Include(l => l.Type)
                .Where(predicate: l => l.Store.Manager.Email == user.Email)
                .ToListAsync();

            // Load the CreatedBy reference for each listing
            foreach (Listing listing in listings)
            {
                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            // Convert the listings to ListingDTO objects and add them to a list
            List<ListingDTO> listDTOs = new List<ListingDTO>();
            foreach (Listing l in listings)
            {
                // Only add listings with the "Approved" or "Received" status to the list
                if (l.Status.Description == "Approved" || l.Status.Description == "Recieved")
                {
                    ListingDTO listingDTO = ConvertToListingDTO(l);
                    listDTOs.Add(listingDTO);
                }
            }

            // Return the list of ListingDTO objects to the view
            return View(listDTOs);
        }



        /// <summary>
        /// Approves the specified listing.
        /// </summary>
        /// <param name="id">The listing's unique identifier.</param>
        /// <returns>The approved listings page.</returns>
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current user
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
                    var user = await GetCurrentUserAsync();

                    // Load the listing from the database and include the Status entity
                    var entity = await _context.Listings
                        .Where(predicate: l => l.Store.Manager.Email == user.Email)
                        .Include(listing => listing.Status)
                        .FirstOrDefaultAsync(l => l.ListingID == id);

                    if (entity != null)
                    {
                        // If the status of the listing is "Unapproved" or "Disapproved", set it to "Approved" and save the changes to the database
                        if (entity.Status.Description == "Unapproved" || entity.Status.Description == "Disapproved")
                        {
                            entity.Status.Description = "Approved";
                            await _context.SaveChangesAsync();
                        }
                    }
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
                return RedirectToAction(nameof(UnapprovedListings));
            }
            return RedirectToAction(nameof(UnapprovedListings));
        }


        /// <summary>
        /// Disapproves the specified listing.
        /// </summary>
        /// <param name="id">The listing's unique identifier.</param>
        /// <returns>The unapproved listings page.</returns>
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Disapprove(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current user
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
                    var user = await GetCurrentUserAsync();

                    // Get the listing with the matching ID and the store's manager email matching the current user's email
                    var entity = await _context.Listings
                        .Where(predicate: l => l.Store.Manager.Email == user.Email)
                        .Include(listing => listing.Status)
                        .FirstOrDefaultAsync(l => l.ListingID == id);

                    if (entity != null)
                    {
                        // Update the status to "Disapproved" if it is currently "Unapproved" or "Approved"
                        if (entity.Status.Description == "Unapproved" || entity.Status.Description == "Approved")
                        {
                            entity.Status.Description = "Disapproved";
                            await _context.SaveChangesAsync();
                        }
                    }
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
                return RedirectToAction(nameof(UnapprovedListings));
            }
            return RedirectToAction(nameof(UnapprovedListings));
        }

        /// <summary>
        /// Marks the specified listing as recieved or picked up.
        /// </summary>
        /// <param name="id">The listing's unique identifier.</param>
        /// <param name="newStatus">The new status of the listing, either "Recieved" or "Picked Up".</param>
        /// <returns>The approved listings page.</returns>
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Recieved_PickedUp(int id)
        {
            // Validate the model state
            if (ModelState.IsValid)
            {
                try
                {
                    // Define a method to get the current user
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

                    // Get the current user
                    var user = await GetCurrentUserAsync();

                    // Get the listing with the specified ID and include the status
                    var entity = await _context.Listings
                        .Where(predicate: l => l.Store.Manager.Email == user.Email)
                        .Include(listing => listing.Status)
                        .FirstOrDefaultAsync(l => l.ListingID == id);

                    // If the listing was found
                    if (entity != null)
                    {
                        // If the listing is approved
                        if (entity.Status.Description == "Approved")
                        {
                            // Change the status to "Recieved" and save the changes
                            entity.Status.Description = "Recieved";
                            await _context.SaveChangesAsync();
                        }
                        // If the listing is recieved
                        else if (entity.Status.Description == "Recieved")
                        {
                            // Change the status to "Picked Up" and save the changes
                            entity.Status.Description = "Picked Up";
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If the listing with the specified ID was not found
                    if (!_context.Listings.Any(e => e.ListingID == id))
                    {
                        // Return a not found result
                        return NotFound();
                    }
                    // Otherwise, throw the exception
                    else
                    {
                        throw;
                    }
                }
                // Redirect to the ApprovedListings action
                return RedirectToAction(nameof(ApprovedListings));
            }
            // Redirect to the ApprovedListings action
            return RedirectToAction(nameof(ApprovedListings));
        }


    }
}
