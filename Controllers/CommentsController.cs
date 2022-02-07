/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AmbroBlogProject.Data;
using AmbroBlogProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AmbroBlogProject.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> OriginalIndex()
        {
            var applicationDbContext = _context.Comments.Include(c => c.BlogUser).Include(c => c.Moderator).Include(c => c.Post);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> ModeratedIndex()
        {
            var moderatedComments = await _context.Comments
                .Where(c => c.Moderated != null)
                .Include(c => c.Post)
                .Include(c => c.BlogUser)
                .ToListAsync();

            ViewData["MainText"] = "Comments";
            ViewData["SubText"] = "All Moderated Comments";

            return View("Index", moderatedComments);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeletedIndex()
        {
            var deletedComments = await _context.Comments
                .Where(c => c.Deleted != null)
                .Include(c => c.Post)
                .Include(c => c.BlogUser)
                .ToListAsync();

            ViewData["MainText"] = "Comments";
            ViewData["SubText"] = "All Deleted Comments";

            return View("Index", deletedComments);
        }

        // GET: Comments
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index()
        {
            var ogComments = _context.Comments.Include(c => c.BlogUser).Include(c => c.Moderator).Include(c => c.Post);

            ViewData["MainText"] = "Comments";
            ViewData["SubText"] = "All Unmoderated Comments";
            return View(await ogComments.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId, Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.BlogUserId = _userManager.GetUserId(User);
                comment.Created = DateTime.Now;

                _context.Add(comment);
                await _context.SaveChangesAsync();

                comment = await _context.Comments
                    .Where(c => c.Id == comment.Id)
                    .Include(c => c.Post)
                    .FirstOrDefaultAsync();

                if (comment == null)
                {
                    return NotFound();
                }

                return RedirectToAction("Details", "Posts", new { Slug = comment.Post.Slug }, "commentSection");
            }

            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = "Administrator,Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", comment.BlogUserId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,BlogUserId,ModeratorId,Body,Created,Updated,Moderated,Deleted,ModeratedBody,ModerationType")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var commentDb = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
                try
                {
                    commentDb.Body = comment.Body;
                    commentDb.Updated = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Details", "Posts", new { slug = commentDb.Post.Slug }, "commentSection");
            }

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Moderate(int id, [Bind("Id, Body, ModeratedBody,ModerationType")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            var commentDb = await _context.Comments
                .Include(c => c.Post)
                .Include(c => c.BlogUser)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    commentDb.ModeratedBody = comment.ModeratedBody;
                    commentDb.ModerationType = comment.ModerationType;
                    commentDb.Moderated = DateTime.Now;
                    commentDb.ModeratorId = _userManager.GetUserId(User);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Details", "Posts", new { slug = commentDb.Post.Slug }, "commentSection");
            }

            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Administrator,Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string slug)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { slug }, "commentSection");
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
