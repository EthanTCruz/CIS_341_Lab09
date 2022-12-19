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

        private readonly ListingsController _listingsController;
        public ManagerController(CommunityStoreContext context, UserManager<ApplicationUser> userManager, ListingsController listingController)
        {
            _context = context;
            _userManager = userManager;
            _listingsController = listingController;

        }

        // GET: Manager
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Index()
        {
            List<Listing> listings = await _context.Listings
                        .Include(listing => listing.Condition)
                        .Include(listing => listing.Store)
                        .Include(listing => listing.ClaimedBy)
                        .Include(listing => listing.Status)
                        .ToListAsync();

            foreach (Listing listing in listings)
            {

                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            List<ListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                ListingDTO listingDTO = _listingsController.ConvertToListingDTO(l);

                listDTOs.Add(listingDTO);
            }

            return View(listDTOs);
        }

        // GET: Manager/Details/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .Include(listing => listing.Condition)
                .Include(listing => listing.Store)
                .Include(listing => listing.CreatedBy)
                .Include(listing => listing.ClaimedBy)
                .Include(listing => listing.Status)
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (listing == null)
            {
                return NotFound();
            }

            ListingDTO listingDTO = _listingsController.ConvertToListingDTO(listing);

            return View(listingDTO);
        }





        // GET: Manager
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UnapprovedListings()
        {
            List<Listing> listings = await _context.Listings
                        .Include(listing => listing.Condition)
                        .Include(listing => listing.Store)
                        .Include(listing => listing.ClaimedBy)
                        .Include(listing => listing.Status)
                        .ToListAsync();

            foreach (Listing listing in listings)
            {

                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            List<ListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                if (l.Status.Description == "Unapproved")
                {
                    ListingDTO listingDTO = _listingsController.ConvertToListingDTO(l);
                    listDTOs.Add(listingDTO);
                }
            }

            return View(listDTOs);
        }



        // GET: Manager
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApprovedListings()
        {
            List<Listing> listings = await _context.Listings
                        .Include(listing => listing.Condition)
                        .Include(listing => listing.Store)
                        .Include(listing => listing.ClaimedBy)
                        .Include(listing => listing.Status)
                        .ToListAsync();

            foreach (Listing listing in listings)
            {

                await _context.Entry(listing).Reference(l => l.CreatedBy).LoadAsync();
            }

            List<ListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                if (l.Status.Description == "Approved" || l.Status.Description == "Recieved")
                {
                    ListingDTO listingDTO = _listingsController.ConvertToListingDTO(l);
                    listDTOs.Add(listingDTO);
                }
            }

            return View(listDTOs);
        }


        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

                    var user = await GetCurrentUserAsync();

                    var entity = await _context.Listings
                        .Where(predicate: l => l.Store.Manager.Email == user.Email)
                        .Include(listing => listing.Status)
                        .FirstOrDefaultAsync(l => l.ListingID == id);

                    if (entity != null)
                    {

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

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Disapprove(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

                    var user = await GetCurrentUserAsync();

                    var entity = await _context.Listings
                        .Where(predicate: l => l.Store.Manager.Email == user.Email)
                        .Include(listing => listing.Status)
                        .FirstOrDefaultAsync(l => l.ListingID == id);

                    if (entity != null)
                    {

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


        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Recieved_PickedUp(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

                    var user = await GetCurrentUserAsync();

                    var entity = await _context.Listings
                        .Where(predicate: l => l.Store.Manager.Email == user.Email)
                        .Include(listing => listing.Status)
                        .FirstOrDefaultAsync(l => l.ListingID == id);

                    if (entity != null)
                    {

                        if (entity.Status.Description == "Approved")
                        {
                            entity.Status.Description = "Recieved";
                            await _context.SaveChangesAsync();
                        }
                        else if (entity.Status.Description == "Recieved")
                        {
                            entity.Status.Description = "Picked Up";
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
                return RedirectToAction(nameof(ApprovedListings));
            }
            return RedirectToAction(nameof(ApprovedListings));
        }

    }
}
