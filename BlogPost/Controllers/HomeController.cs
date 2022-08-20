using NovaBlog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NovaBlog.Data;
using Microsoft.EntityFrameworkCore;
using NovaBlog.Services.Interfaces;
using X.PagedList;

namespace NovaBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogPostService blogPostService)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
        }

        public async Task<IActionResult> AuthorPage(int? pageNum)
        {            
            int pageSize = 4;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogPostService.GetAllBlogPostsAsync()).Where(b => b.IsPublished == true)
                                                          .ToPagedListAsync(page, pageSize);

            //List<BlogPost> posts = (await _blogPostService.GetAllBlogPostsAsync()).Where(b => b.IsPublished == true).ToList();

            return View(blogPosts);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> BlogPostsCategoryFilter(int categoryId)
        {
            List<BlogPost> posts = (await _blogPostService.GetAllBlogPostsAsync()).Where(b => b.IsPublished == true && b.CategoryId == categoryId).ToList();
            return View(posts);
        }

        public async Task<IActionResult> BlogPostTagFilter(int tagId)
        {
            Tag? tag = await _context.Tags.FindAsync(tagId);
            List<BlogPost> posts = (await _blogPostService.GetAllBlogPostsAsync()).Where(b => b.IsPublished == true)
                                                                                    .Where(b => b.Tags.Contains(tag!)).ToList();
            return View(posts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}