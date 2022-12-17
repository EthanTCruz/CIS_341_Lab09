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
    public class ManagerController : Controller
    {
        private readonly CommunityStoreContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public ManagerController(CommunityStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                ListingDTO listingDTO = new()
                {
                    ListingID = l.ListingID,
                    Quantity = l.Quantity,
                    Description = l.Description,
                    CreatedBy = l.CreatedBy.Name,
                    Status = l.Status.Description,
                    Store = l.Store.Name,
                    Condition = l.Condition.Description
                };
                listDTOs.Add(listingDTO);
            }

            return View(listDTOs);
        }

        // GET: Manager/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ListingDTO == null)
            {
                return NotFound();
            }

            var ListingDTO = await _context.ListingDTO
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (ListingDTO == null)
            {
                return NotFound();
            }

            return View(ListingDTO);
        }







        // GET: Manager/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ListingDTO == null)
            {
                return NotFound();
            }

            var ListingDTO = await _context.ListingDTO
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (ListingDTO == null)
            {
                return NotFound();
            }

            return View(ListingDTO);
        }

        // POST: Manager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ListingDTO == null)
            {
                return Problem("Entity set 'CommunityStoreContext.ListingDTO'  is null.");
            }
            var ListingDTO = await _context.ListingDTO.FindAsync(id);
            if (ListingDTO != null)
            {
                _context.ListingDTO.Remove(ListingDTO);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListingDTOExists(int id)
        {
          return _context.ListingDTO.Any(e => e.ListingID == id);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Approve_Unapprove(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

                    var user = await GetCurrentUserAsync();

                    var entity = await _context.Listings
                        .Where(predicate: l => l.Store.Manager.Email == user.Email)
                        .FirstOrDefaultAsync(l => l.ListingID == id);

                    if (entity != null)
                    {

                        if (entity.Status.Description == "Unapproved")
                        {
                            entity.Status.Description = "Approved";
                        }
                        else
                        {
                            entity.Status.Description = "Unapproved";
                        }
                        await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
