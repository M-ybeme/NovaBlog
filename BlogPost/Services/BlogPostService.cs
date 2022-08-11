using Microsoft.EntityFrameworkCore;
using NovaBlog.Data;
using NovaBlog.Extensions;
using NovaBlog.Models;
using NovaBlog.Services.Interfaces;

namespace NovaBlog.Services
{


    
    public class BlogPostService : IBlogPostService
    {
        readonly ApplicationDbContext _context;

        public BlogPostService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            try
            {
                string newSlug = title.Slugify();

                if(blogId == 0)
                {
                    return !(await _context.BlogPosts.AnyAsync(b=> b.Slug == newSlug));
                }
                else
                {
                    BlogPost blogPost = await _context.BlogPosts.AsNoTracking().FirstAsync(b=>b.Id == blogId);

                    string oldSlug = blogPost.Slug!;

                    if(!string.Equals(oldSlug, newSlug))
                    {
                        return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                    }
                }

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
