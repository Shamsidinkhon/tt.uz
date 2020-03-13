using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tt.admin.Data;
using tt.uz.Entities;

namespace tt.admin.Controllers
{
    [Authorize]
    public class CoreAttributesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoreAttributesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CoreAttributes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CoreAttribute.ToListAsync());
        }

        // GET: CoreAttributes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coreAttribute = await _context.CoreAttribute
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coreAttribute == null)
            {
                return NotFound();
            }

            return View(coreAttribute);
        }

        // GET: CoreAttributes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CoreAttributes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Title,Type,Unit,Required")] CoreAttribute coreAttribute)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coreAttribute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coreAttribute);
        }

        // GET: CoreAttributes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coreAttribute = await _context.CoreAttribute.FindAsync(id);
            if (coreAttribute == null)
            {
                return NotFound();
            }
            return View(coreAttribute);
        }

        // POST: CoreAttributes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Title,Type,Unit,Required")] CoreAttribute coreAttribute)
        {
            if (id != coreAttribute.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coreAttribute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoreAttributeExists(coreAttribute.Id))
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
            return View(coreAttribute);
        }

        // GET: CoreAttributes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coreAttribute = await _context.CoreAttribute
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coreAttribute == null)
            {
                return NotFound();
            }

            return View(coreAttribute);
        }

        // POST: CoreAttributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coreAttribute = await _context.CoreAttribute.FindAsync(id);
            _context.CoreAttribute.Remove(coreAttribute);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoreAttributeExists(int id)
        {
            return _context.CoreAttribute.Any(e => e.Id == id);
        }
    }
}
