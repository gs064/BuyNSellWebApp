using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BuyNSellWebApp.Data;
using BuyNSellWebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace BuyNSellWebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class SubTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubTypes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SubTypes.Include(s => s.Types);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SubTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subType = await _context.SubTypes
                .Include(s => s.Types)
                .FirstOrDefaultAsync(m => m.SubTypeID == id);
            if (subType == null)
            {
                return NotFound();
            }

            return View(subType);
        }

        // GET: SubTypes/Create
        public IActionResult Create()
        {
            ViewData["TypeID"] = new SelectList(_context.Types, "TypeID", "TypeName");
            return View();
        }

        // POST: SubTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubTypeID,SubTypeName,TypeID")] SubType subType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeID"] = new SelectList(_context.Types, "TypeID", "TypeName", subType.TypeID);
            return View(subType);
        }

        // GET: SubTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subType = await _context.SubTypes.FindAsync(id);
            if (subType == null)
            {
                return NotFound();
            }
            ViewData["TypeID"] = new SelectList(_context.Types, "TypeID", "TypeName", subType.TypeID);
            return View(subType);
        }

        // POST: SubTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubTypeID,SubTypeName,TypeID")] SubType subType)
        {
            if (id != subType.SubTypeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubTypeExists(subType.SubTypeID))
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
            ViewData["TypeID"] = new SelectList(_context.Types, "TypeID", "TypeName", subType.TypeID);
            return View(subType);
        }

        // GET: SubTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subType = await _context.SubTypes
                .Include(s => s.Types)
                .FirstOrDefaultAsync(m => m.SubTypeID == id);
            if (subType == null)
            {
                return NotFound();
            }

            return View(subType);
        }

        // POST: SubTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subType = await _context.SubTypes.FindAsync(id);
            _context.SubTypes.Remove(subType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubTypeExists(int id)
        {
            return _context.SubTypes.Any(e => e.SubTypeID == id);
        }
    }
}
