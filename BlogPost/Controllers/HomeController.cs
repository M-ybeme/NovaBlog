﻿using NovaBlog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NovaBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace NovaBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> AuthorPage()
        {
            //To Do: create servcie to get blogPosts

            List<BlogPost> posts = await _context.BlogPosts
                                    .Include(b => b.Comments)
                                    .Include(b => b.Category)
                                    .Include(b => b.Tags)
                                    .ToListAsync();


            return View(posts);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}