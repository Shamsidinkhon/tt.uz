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
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.News.Include(n => n.Category).Include(n => n.ContactDetail).Include(n => n.Location).Include(n => n.Price);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Category)
                .Include(n => n.ContactDetail)
                .Include(n => n.Location)
                .Include(n => n.Price)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Id");
            ViewData["ContactDetailId"] = new SelectList(_context.Set<ContactDetail>(), "ContactDetailId", "ContactDetailId");
            ViewData["LocationId"] = new SelectList(_context.Set<Location>(), "LocationId", "LocationId");
            ViewData["PriceId"] = new SelectList(_context.Set<Price>(), "PriceId", "PriceId");
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,CategoryId,PriceId,Description,LocationId,ContactDetailId,OwnerId,Status,CreatedDate,UpdatedDate")] News news)
        {
            if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Id", news.CategoryId);
            ViewData["ContactDetailId"] = new SelectList(_context.Set<ContactDetail>(), "ContactDetailId", "ContactDetailId", news.ContactDetailId);
            ViewData["LocationId"] = new SelectList(_context.Set<Location>(), "LocationId", "LocationId", news.LocationId);
            ViewData["PriceId"] = new SelectList(_context.Set<Price>(), "PriceId", "PriceId", news.PriceId);
            return View(news);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Id", news.CategoryId);
            ViewData["ContactDetailId"] = new SelectList(_context.Set<ContactDetail>(), "ContactDetailId", "ContactDetailId", news.ContactDetailId);
            ViewData["LocationId"] = new SelectList(_context.Set<Location>(), "LocationId", "LocationId", news.LocationId);
            ViewData["PriceId"] = new SelectList(_context.Set<Price>(), "PriceId", "PriceId", news.PriceId);
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,CategoryId,PriceId,Description,LocationId,ContactDetailId,OwnerId,Status,CreatedDate,UpdatedDate")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Id", news.CategoryId);
            ViewData["ContactDetailId"] = new SelectList(_context.Set<ContactDetail>(), "ContactDetailId", "ContactDetailId", news.ContactDetailId);
            ViewData["LocationId"] = new SelectList(_context.Set<Location>(), "LocationId", "LocationId", news.LocationId);
            ViewData["PriceId"] = new SelectList(_context.Set<Price>(), "PriceId", "PriceId", news.PriceId);
            return View(news);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Category)
                .Include(n => n.ContactDetail)
                .Include(n => n.Location)
                .Include(n => n.Price)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
