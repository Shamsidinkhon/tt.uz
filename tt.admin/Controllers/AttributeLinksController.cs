using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tt.admin.Data;
using tt.admin.Models;
using tt.uz.Entities;

namespace tt.admin.Controllers
{
    [Authorize]
    public class AttributeLinksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttributeLinksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttributeLinks
        public ActionResult Index()
        {
            var groups = from l in _context.AttributeLink

                         join c in _context.CoreAttribute on l.AttributeId equals c.Id
                         into ca
                         from c in ca

                         join cat in _context.Categories on l.TypeId equals cat.AttributeType
                         into category
                         from cat in category.DefaultIfEmpty()

                         group new { Attibutes = ca, Categories = category } by l.TypeId into grouped
                         orderby grouped.Key
                         select new AttributeLinkViewModel() {
                             Type = grouped.Key,
                             Attributes = grouped.SelectMany(x => x.Attibutes).Distinct().ToList(),
                             Categories = grouped.SelectMany(x => x.Categories).Distinct().ToList()
                         };

            return View(groups.ToList());
        }

        // GET: AttributeLinks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attributeLink = await _context.AttributeLink
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attributeLink == null)
            {
                return NotFound();
            }

            return View(attributeLink);
        }

        // GET: AttributeLinks/Create
        public IActionResult Create()
        {
            ViewData["AttributeId"] = new SelectList(_context.CoreAttribute, "Id", "Title");
            return View();
        }

        public IActionResult AddCategory(int Id)
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            CategoryPostModel model = new CategoryPostModel
            {
                TypeId = Id
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory([Bind("CategoryId,TypeId")] CategoryPostModel model)
        {
            if (ModelState.IsValid)
            {
                var cat = _context.Categories.SingleOrDefault(x => x.Id == model.CategoryId);
                if (cat != null)
                {
                    cat.AttributeType = model.TypeId;
                    _context.Categories.Update(cat);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: AttributeLinks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypeId,AttributeId")] AttributeLink attributeLink)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attributeLink);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(attributeLink);
        }

        // GET: AttributeLinks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attributeLink = await _context.AttributeLink.FindAsync(id);
            if (attributeLink == null)
            {
                return NotFound();
            }
            return View(attributeLink);
        }

        // POST: AttributeLinks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeId,AttributeId")] AttributeLink attributeLink)
        {
            if (id != attributeLink.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attributeLink);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttributeLinkExists(attributeLink.Id))
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
            return View(attributeLink);
        }

        // GET: AttributeLinks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attributeLink = await _context.AttributeLink
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attributeLink == null)
            {
                return NotFound();
            }

            return View(attributeLink);
        }

        // POST: AttributeLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attributeLink = await _context.AttributeLink.FindAsync(id);
            _context.AttributeLink.Remove(attributeLink);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, ActionName("DeleteCategoryType")]
        public ActionResult DeleteCategoryType(int id)
        {
            var cat = _context.Categories.SingleOrDefault(x => x.Id == id);
            if (cat != null)
            {
                cat.AttributeType = 0;
                _context.Categories.Update(cat);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AttributeLinkExists(int id)
        {
            return _context.AttributeLink.Any(e => e.Id == id);
        }
    }
}
