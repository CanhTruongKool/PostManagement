using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostManagement.Models;

namespace _29_PhamThiNgocMai_Assignment3.Controllers
{
    public class PostCategoriesController : Controller
    {
        private readonly PostManagementDbContext _context;

        public PostCategoriesController(PostManagementDbContext context)
        {
            _context = context;
        }

        // GET: PostCategories
        public async Task<IActionResult> Index()
        {
            return _context.PostCategories != null ?
                        View(await _context.PostCategories.ToListAsync()) :
                        Problem("Entity set PostCategory  is null.");
        }

        // GET: PostCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PostCategories == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // GET: PostCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PostCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Description")] PostCategories postCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(postCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postCategory);
        }

        // GET: PostCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PostCategories == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories.FindAsync(id);
            if (postCategory == null)
            {
                return NotFound();
            }
            return View(postCategory);
        }

        // POST: PostCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Description")] PostCategories postCategory)
        {
            if (id != postCategory.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostCategoryExists(postCategory.CategoryId))
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
            return View(postCategory);
        }

        // GET: PostCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PostCategories == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // POST: PostCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PostCategories == null)
            {
                return Problem("Entity set '_29_PhamThiNgocMai_Assignment3Context.PostCategory'  is null.");
            }
            var postCategory = await _context.PostCategories.FindAsync(id);
            if (postCategory != null)
            {
                _context.PostCategories.Remove(postCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostCategoryExists(int id)
        {
            return (_context.PostCategories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}