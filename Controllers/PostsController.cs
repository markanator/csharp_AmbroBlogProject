﻿/*
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
using AmbroBlogProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using X.PagedList;

namespace AmbroBlogProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;

        public PostsController(ApplicationDbContext context, ISlugService slugService, IImageService imageService, UserManager<BlogUser> userManager, BlogSearchService blogSearchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
        }

        // GET: SearchIdex/{page}?searhTerm
        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 5;

            var posts = _blogSearchService.Search(searchTerm);
            var blogs = await _context.Blogs
                            .Include(b => b.BlogUser)
                            .OrderByDescending(b => b.Created)
                            .ToListAsync();

            ViewData["Categories"] = blogs;

            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Posts
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/BlogPostIndex/{id}
        public async Task<IActionResult> BlogPostIndex(int? id, int? page)
        {
            if (id == null) return NotFound();

            var pageNum = page ?? 1;
            var pageSize = 5;

            var posts = await _context.Posts
                .Where(p => p.BlogId == id && p.ReadyStatus == Enums.ReadyStatus.ProductionReady)
                .OrderBy(p => p.Created)
                .ToPagedListAsync(pageNum, pageSize);

            var blog = await _context.Blogs
                .FindAsync(id);

            var blogs = await _context.Blogs
                            .Include(b => b.BlogUser)
                            .OrderByDescending(b => b.Created)
                            .ToListAsync();

            ViewData["Categories"] = blogs;

            ViewData["MainText"] = blog.Name;
            ViewData["SubText"] = blog.Description;
            ViewData["BlogTitle"] = $"{blog.Name}";

            return View(posts);
        }

        // GET: Posts/TagIndex?tag={tag}&page={n}
        public async Task<IActionResult> TagIndex(string tag, int? page)
        {
            var pageNum = page ?? 1;
            var pageSize = 6;

            var allPostIds = _context.Tags.Where(t => t.Text == tag).Select(t => t.PostId);

            var posts = await _context.Posts.Where(p => allPostIds
                .Contains(p.Id))
                .OrderByDescending(t => t.Created)
                .ToPagedListAsync(pageNum, pageSize);

            var blogs = await _context.Blogs
                            .Include(b => b.BlogUser)
                            .OrderByDescending(b => b.Created)
                            .ToListAsync();

            ViewData["Categories"] = blogs;
            ViewData["TagTerm"] = $"Results for \"{tag}\"";

            return View(posts);
        }

        // GET: Posts/Details/[Slug]
        public async Task<IActionResult> Details(string slug)
        {
            ViewData["Title"] = "Post Details Page";
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var post = await _context.Posts
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)// connected to Comments
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null) return NotFound();

            var dataVM = new ViewModels.PostDetailViewModel()
            {
                Post = post,
                Tags = _context.Tags.Select(t => t.Text.ToLower()).Distinct().ToList(),
            };

            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageDate, post.ContentType);
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;

            return View(dataVM);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["MainText"] = "Create Blog";
            ViewData["SubText"] = "Create a new Blog";
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image, isFeatured")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                // create slug and check for unique
                string slug = _slugService.UrlFriendly(post.Title);
                bool validationError = false;

                if (string.IsNullOrEmpty(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("", "The title provided cannnot be used as it results in an empty slug.");
                }
                else if (!_slugService.IsUnique(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("Title", "The title you provided cannot be used. Duplicate.");
                }
                else if (validationError)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                var blogUserId = _userManager.GetUserId(User);
                post.BlogUserId = blogUserId;
                post.Slug = slug;
                post.Created = DateTime.Now;
                post.ImageDate = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);

                _context.Add(post);
                await _context.SaveChangesAsync();

                foreach (var tag in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = blogUserId,
                        Text = tag
                    });
                }
                await _context.SaveChangesAsync(); // store new listing of tags, with Post Id
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);

            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));

            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus, isFeatured")] Post post, IFormFile newImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var editedPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);

                    // general updated fields
                    editedPost.Updated = DateTime.Now;
                    editedPost.Abstract = post.Abstract;
                    editedPost.Content = post.Content;
                    editedPost.ReadyStatus = post.ReadyStatus;
                    editedPost.BlogId = post.BlogId;
                    editedPost.isFeatured = post.isFeatured;

                    var newSlug = _slugService.UrlFriendly(post.Title);
                    if (newSlug != editedPost.Slug)
                    {
                        if (_slugService.IsUnique(newSlug))
                        {
                            editedPost.Title = post.Title;
                            editedPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This title cannot be used as it results in a duplicate slug.");
                            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
                            return View(post);
                        }
                    }

                    // Post Image
                    if (newImage != null)
                    {
                        editedPost.ImageDate = await _imageService.EncodeImageAsync(newImage);
                        editedPost.ContentType = _imageService.ContentType(newImage);
                    }

                    // Post Tags
                    _context.Tags.RemoveRange(editedPost.Tags);

                    foreach (var tag in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = editedPost.BlogUserId,
                            Text = tag
                        });
                    }


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
