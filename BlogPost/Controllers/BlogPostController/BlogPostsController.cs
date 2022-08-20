using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NovaBlog.Data;
using NovaBlog.Extensions;
using NovaBlog.Models;
using NovaBlog.Services;
using NovaBlog.Services.Interfaces;
using X.PagedList;

namespace NovaBlog.Controllers.BlogPostController
{
    [Authorize(Roles = "Administrator")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IBlogPostService _blogPostService;

        public BlogPostsController(ApplicationDbContext context, 
                                   IImageService imageService,
                                   IBlogPostService blogPostService)
        {
            _context = context;
            _imageService = imageService;
            _blogPostService = blogPostService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            //TODO: Use service

            var applicationDbContext = _context.BlogPosts
                                               .Include(b => b.Category)
                                               .Include(b=>b.Tags)
                                               .Where(b => b.IsDeleted == false);

            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> SearchIndex(string searchTerm, int? pageNum)
        {

            int pageSize = 4;
            int page = pageNum ?? 1;

            ViewData["SearchTerm"] = searchTerm;

            IPagedList<BlogPost> blogPosts = await _blogPostService.Search(searchTerm).ToPagedListAsync(page, pageSize);

            return View(blogPosts);
        }

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            BlogPost? blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.Author)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (blogPost == null)
            {
                return NotFound();
            }
            
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["TagsList"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Contnet,CategoryId,Slug,Abstract,IsPublished,BlogPostImage")] BlogPost blogPost, List<int> Tags)
        {
            if (ModelState.IsValid)
            {
                //Date(s)
                blogPost.Created = DataUtility.GetPostgresDate(DateTime.Now);
                //slug

                if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!,blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title or slug has already been used!");

                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                    ViewData["TagsList"] = new MultiSelectList(_context.Tags, "Id", "Name");

                    return View(blogPost);
                }
                blogPost.Slug = blogPost.Title!.Slugify();

                //check if image is null
                if (blogPost.BlogPostImage != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                    blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                }


            
                //post tags to blogPost
                foreach(int tagId in Tags)
                {
                    blogPost.Tags.Add(_context.Tags.Find(tagId)!);
                }


                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagsList"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags.Select(t =>t.Id));

            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Contnet,Created,LastUpdated,CategoryId,ImageData,ImageType,Slug,Abstract,IsPublished,IsDeleted,BlogPostImage")] BlogPost blogPost, List<int> TagList)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Dates Example
                    blogPost.Created = DataUtility.GetPostgresDate(blogPost.Created);
                    blogPost.LastUpdated = DataUtility.GetPostgresDate(DateTime.Now);

                    //image check
                    if (blogPost.BlogPostImage != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                        blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                    }
                    //slug 
                    if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title or slug has already been used!");

                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
                        ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name", TagList);

                        return View(blogPost);
                    }
                    blogPost.Slug = blogPost.Title!.Slugify();


                    _context.Update(blogPost);
                    
                    await _context.SaveChangesAsync();

                    //create list of old tags
                    List<Tag> oldTtags = _context.Tags.Where(t => t.BlogPosts.Contains(blogPost)).ToList();
                    //


                    //blogPost.Tags.Clear();

                    foreach ( Tag tag in oldTtags)
                    {
                        await _blogPostService.RemoveTagFromBlogPostAsync(tag.Id, blogPost.Id);
                        //blogPost.Tags.Remove(tag);
                    }
                    

                    foreach (int tagId in TagList)
                    {
                        await _blogPostService.AddTagToBlogPostAsync(tagId, blogPost.Id);
                        //blogPost.Tags.Add(_context.Tags.Find(tagId)!);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagsList"] = new MultiSelectList(_context.Tags, "Id", "Name", TagList);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            //perma delete
            //if (blogPost != null)
            //{
            //    _context.BlogPosts.Remove(blogPost);
            //}
            blogPost!.IsDeleted = true;

            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
          return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
