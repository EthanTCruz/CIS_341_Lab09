using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab8.Data;
using Lab8.Models.DTO;

namespace Lab8.Controllers
{
    public class ListingDTOesController : Controller
    {
        private readonly CommunityStoreContext _context;

        public ListingDTOesController(CommunityStoreContext context)
        {
            _context = context;
        }

        // GET: ListingDTOes
        public async Task<IActionResult> Index()
        {
              return View(await _context.ListingDTO.ToListAsync());
        }

        // GET: ListingDTOes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ListingDTO == null)
            {
                return NotFound();
            }

            var listingDTO = await _context.ListingDTO
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (listingDTO == null)
            {
                return NotFound();
            }

            return View(listingDTO);
        }

        // GET: ListingDTOes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ListingDTOes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListingID,Condition,Description,Quantity,CreatedBy,ClaimedBy,Store,Status")] ListingDTO listingDTO)
        {
            if (ModelState.IsValid)
            {
                _context.Add(listingDTO);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(listingDTO);
        }

        // GET: ListingDTOes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ListingDTO == null)
            {
                return NotFound();
            }

            var listingDTO = await _context.ListingDTO.FindAsync(id);
            if (listingDTO == null)
            {
                return NotFound();
            }
            return View(listingDTO);
        }

        // POST: ListingDTOes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ListingID,Condition,Description,Quantity,CreatedBy,ClaimedBy,Store,Status")] ListingDTO listingDTO)
        {
            if (id != listingDTO.ListingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listingDTO);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListingDTOExists(listingDTO.ListingID))
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
            return View(listingDTO);
        }

        // GET: ListingDTOes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ListingDTO == null)
            {
                return NotFound();
            }

            var listingDTO = await _context.ListingDTO
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (listingDTO == null)
            {
                return NotFound();
            }

            return View(listingDTO);
        }

        // POST: ListingDTOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ListingDTO == null)
            {
                return Problem("Entity set 'CommunityStoreContext.ListingDTO'  is null.");
            }
            var listingDTO = await _context.ListingDTO.FindAsync(id);
            if (listingDTO != null)
            {
                _context.ListingDTO.Remove(listingDTO);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListingDTOExists(int id)
        {
          return _context.ListingDTO.Any(e => e.ListingID == id);
        }
    }
}
