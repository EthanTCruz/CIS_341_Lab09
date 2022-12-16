using Lab8.Models.DTO;
using Lab8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Lab8.Areas.Identity.Data;
using Lab8.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            // GET: ManagerController
            [Authorize(Roles = "Manager")]
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

            List<ManagerListingDTO> listDTOs = new();
            foreach (Listing l in listings)
            {
                ManagerListingDTO listingDTO = new()
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

        // GET: ManagerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ManagerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ManagerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ManagerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ManagerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ManagerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
