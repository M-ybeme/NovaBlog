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

                if (blogId == 0)
                {
                    return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                }
                else
                {
                    BlogPost blogPost = await _context.BlogPosts.AsNoTracking().FirstAsync(b => b.Id == blogId);

                    string oldSlug = blogPost.Slug!;

                    if (!string.Equals(oldSlug, newSlug))
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

        public async Task RemoveTagFromBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                if (await IsTagOnBlogPostAsync(tagId, blogPostId))
                {
                    BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        blogPost.Tags.Remove(tag);
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

                return (await _context.BlogPosts.Include(t => t.Tags)
                .FirstOrDefaultAsync(b => b.Id == blogPostId))!.Tags.Contains(tag!);

            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = new List<Category>();

            try
            {
                categories = await _context.Categories.Include(c => c.BlogPosts).ToListAsync();
            }
            catch
            {

                throw;
            }
            return categories;
        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            List<BlogPost> blogPosts = new List<BlogPost>();

            try
            {
                blogPosts = await _context.BlogPosts
                                                    .Where(b => b.IsDeleted == false)
                                                    .Include(b => b.Comments)
                                                        .ThenInclude(c => c.Author)
                                                    .Include(b => b.Category)
                                                    .Include(b => b. Tags)
                                                    .ToListAsync();
            }
            catch 
            {

                throw;
            }
            return blogPosts;
        }

        public async Task<List<BlogPost>> GetPopularBlogPostsAsync(int count)
        {
            List<BlogPost> blogPosts = new List<BlogPost>();
            try
            {
                
                blogPosts = await _context.BlogPosts.OrderByDescending(b => b.Comments.Count)
                                                    .Include(b => b.Comments)
                                                        .ThenInclude(c => c.Author)
                                                    .Include(b => b.Tags)
                                                    .Include(b => b.Category)     
                                                    .Take(count)
                                                    .ToListAsync();
            }
            catch
            {

                throw;
            }
            return blogPosts;
        }

        public async Task<List<BlogPost>> GetRecentBlogPostsAsync(int count)
        {
            List<BlogPost> blogPosts = new List<BlogPost>();
            try
            {
                blogPosts = await _context.BlogPosts.OrderByDescending(b => b.Created)
                                                    .Include(b => b.Category)
                                                    .Include(b => b.Comments)
                                                        .ThenInclude(c => c.Author)
                                                    .Include(b => b.Tags)
                                                    .Take(count)
                                                    .ToListAsync();
            }
            catch 
            {

                throw;
            }
            return blogPosts;

        }

        public IEnumerable<BlogPost> Search(string searchString)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();
                if (string.IsNullOrWhiteSpace(searchString))
                {
                    return blogPosts;
                }
                else
                {
                    //search queries in db
                    searchString = searchString.Trim().ToLower();

                    blogPosts = _context.BlogPosts.Where(b => b.Title!.ToLower().Contains(searchString) ||
                                                              b.Abstract!.ToLower().Contains(searchString) ||
                                                              b.Contnet!.ToLower().Contains(searchString) ||
                                                              b.Category!.Name!.ToLower().Contains(searchString) ||
                                                              b.Comments.Any(
                                                                  c => c.Body!.ToLower().Contains(searchString) ||
                                                                       c.Author!.FirstName!.ToLower().Contains(searchString) ||
                                                                       c.Author.LastName!.ToLower().Contains(searchString)) ||
                                                              b.Tags.Any(t => t.Name!.ToLower().Contains(searchString)))
                                                   .Include(b => b.Comments)
                                                        .ThenInclude(c => c.Author)
                                                   .Include(b => b.Tags)
                                                   .Include(b => b.Category)
                                                   .Where(b => b.IsDeleted == false && b.IsPublished == true)
                                                   .AsNoTracking()
                                                   .OrderByDescending(b => b.Created)
                                                   .AsEnumerable();

                    return blogPosts;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            List<Tag> tags = new List<Tag>();

            try
            {
                tags = await _context.Tags.Include(c => c.BlogPosts).ToListAsync();
                return tags;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
