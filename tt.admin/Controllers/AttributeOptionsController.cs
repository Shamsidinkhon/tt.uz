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
    public class AttributeOptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttributeOptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttributeOptions
        public async Task<IActionResult> Index()
        {
            return View(await _context.AttributeOption.ToListAsync());
        }

        // GET: AttributeOptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attributeOption = await _context.AttributeOption
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attributeOption == null)
            {
                return NotFound();
            }

            return View(attributeOption);
        }

        // GET: AttributeOptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AttributeOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AttributeId,Position,Value")] AttributeOption attributeOption)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attributeOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(attributeOption);
        }

        // GET: AttributeOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attributeOption = await _context.AttributeOption.FindAsync(id);
            if (attributeOption == null)
            {
                return NotFound();
            }
            return View(attributeOption);
        }

        // POST: AttributeOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AttributeId,Position,Value")] AttributeOption attributeOption)
        {
            if (id != attributeOption.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attributeOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttributeOptionExists(attributeOption.Id))
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
            return View(attributeOption);
        }

        // GET: AttributeOptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attributeOption = await _context.AttributeOption
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attributeOption == null)
            {
                return NotFound();
            }

            return View(attributeOption);
        }

        // POST: AttributeOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attributeOption = await _context.AttributeOption.FindAsync(id);
            _context.AttributeOption.Remove(attributeOption);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttributeOptionExists(int id)
        {
            return _context.AttributeOption.Any(e => e.Id == id);
        }
    }
}
