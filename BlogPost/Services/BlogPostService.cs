using Microsoft.EntityFrameworkCore;
using NovaBlog.Data;
using NovaBlog.Extensions;
using NovaBlog.Models;
using NovaBlog.Services.Interfaces;
using System.Linq;

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

        public async Task AddTagToBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                //see if category has already been added
                if (!await IsTagOnBlogPostAsync(tagId, blogPostId))
                {
                    //if not then add to Database
                    BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        blogPost.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }
                }

            }
            catch
            {
                throw;
            }


        }
            public async Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId)
            {
                try
                {
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    return (await _context.BlogPosts.FirstOrDefaultAsync(b => b.Id == blogPostId))!.Tags.Contains(tag!);
                                        
                }
                catch
                {
                    throw;
                }
            }
    }
}
