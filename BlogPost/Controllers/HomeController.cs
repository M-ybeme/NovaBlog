using NovaBlog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NovaBlog.Data;
using Microsoft.EntityFrameworkCore;
using NovaBlog.Services.Interfaces;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NovaBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<BlogUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogPostService blogPostService, IEmailSender emailSender, UserManager<BlogUser> userManager)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> AuthorPage(int? pageNum)
        {
            int pageSize = 4;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogPostService.GetAllBlogPostsAsync())
                                                                          .Where(b => b.IsPublished == true)
                                                                          .OrderByDescending(b => b.Created)
                                                                          .ToPagedListAsync(page, pageSize);

            //List<BlogPost> posts = (await _blogPostService.GetAllBlogPostsAsync()).Where(b => b.IsPublished == true).ToList();

            return View(blogPosts);
        }
        //Get
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ContactMeAsync()
        {
            EmailData emailData = new();
            emailData.UserEmail = (await _userManager.GetUserAsync(User))?.Email;
            return View(emailData);
        }

        //Post
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(EmailData emailData)
        {

            BlogUser blogUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {

                var adminEmail = "MarloMayberry.90@gmail.com";

                emailData.Body += $@"
                <br><hr/>
                Contact info:
                <tr>
                    <td>Name: </td>
                    <dtd>{blogUser.FullName}</td>
                </tr>
                <tr>
                    <td>Email: </td>
                    <td>{blogUser.Email}</td>
                <tr>";


                try
                {
                    await _emailSender.SendEmailAsync(adminEmail, emailData.Subject, emailData.Body);
                    return RedirectToAction("ContactMe", "Home", new {swalMessage = "Success: Email Sent!" });
                }
                catch
                {
                    return RedirectToAction("ContactMe", "Home", new { swalMessage = "Error: Email Send Failed!" });
                    throw;
                }
            }

            return View(emailData);
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