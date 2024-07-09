using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PostManagement.Models;
using PostManagement.Services;

namespace PostManagement.Controllers
{
    public class PostsController : Controller
    {
        private readonly PostManagementDbContext _context;
        private readonly IHubContext<SignalRServer> _signalRHub;
        private readonly IConfiguration Configuration;

        public PostsController(PostManagementDbContext context
            , IHubContext<SignalRServer> signalRHub,
            IConfiguration configuration)
        {
            _context = context;
            _signalRHub = signalRHub;
            Configuration = configuration;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string searchTitle, string searchContent,
            string searchStartDate, string searchEndDate)
        {
            DateTime minDate = Convert.ToDateTime(searchStartDate).Date;
            DateTime maxDate = Convert.ToDateTime(searchEndDate).Date;

            List<Posts> posts = await
                _context.Posts
                    .Where(p => p.PublishStatus == 1)
                    .Include(p => p.Users)
                    .Include(p => p.PostCategories)
                    .OrderByDescending(x => x.CreateDate)
                    .ThenByDescending(x => x.UpdateDate)
                    .ToListAsync();

            if (!String.IsNullOrEmpty(searchTitle))
            {
                posts = posts.Where(p => p.Title.ToLower().Trim()
                    .Contains(searchTitle.ToLower().Trim())).ToList();
            }

            if (!String.IsNullOrEmpty(searchContent))
            {
                posts = posts.Where(p => p.Content.ToLower().Trim()
                    .Contains(searchContent.ToLower().Trim())).ToList();
            }

            if (!String.IsNullOrEmpty(searchStartDate))
            {
                posts = posts.Where(p => p.CreateDate.Date >= minDate.Date).ToList();
            }

            if (!String.IsNullOrEmpty(searchEndDate))
            {
                posts = posts.Where(p => p.CreateDate.Date <= maxDate.Date).ToList();
            }

            ViewData["searchTitle"] = searchTitle;
            ViewData["searchContent"] = searchContent;
            ViewData["searchStartDate"] = searchStartDate;
            ViewData["searchEndDate"] = searchEndDate;

            return View(posts);
        }

        [HttpGet]
        public IActionResult GetPosts()
        {
            var res = _context.Posts
                .Include(p => p.Users)
                .Include(p => p.PostCategories)
                .OrderByDescending(x => x.CreateDate)
                .ThenByDescending(x => x.UpdateDate)
                .ToList();
            return Ok(res);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Users)
                .Include(p => p.PostCategories)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "Fullname");
            ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PostId,AuthorId,Title,Content,PublishStatus,CatergoryId")]
            Posts post)
        {
            if (ModelState.IsValid)
            {
                post.CreateDate = DateTime.Now;
                post.UpdateDate = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();
                await _signalRHub.Clients.All.SendAsync("LoadPosts");
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "Fullname", post.AuthorId);
            ViewData["CategoryId"] =
                new SelectList(_context.PostCategories, "CategoryId", "CategoryName", post.CatergoryId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Where(item => item.PostId == id).FirstOrDefaultAsync();
            if (post == null)
            {
                return NotFound();
            }

            ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "FullName", post.AuthorId);
            ViewData["CatergoryId"] =
                new SelectList(_context.PostCategories, "CategoryId", "CategoryName", post.CatergoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int PostId,
            [Bind("PostId,AuthorId,CreatedDate,Title,Content,PublishStatus,CatergoryId")]
            Posts post)
        {
            if (PostId != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.UpdateDate = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    await _signalRHub.Clients.All.SendAsync("LoadPosts");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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

            ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "Fullname", post.AuthorId);
            ViewData["CategoryId"] =
                new SelectList(_context.PostCategories, "CategoryId", "CategoryName", post.CatergoryId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Users)
                .Include(p => p.PostCategories)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int PostId)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set Post is null.");
            }

            var post = await _context.Posts.FindAsync(PostId);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            await _signalRHub.Clients.All.SendAsync("LoadPosts");
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }

}
