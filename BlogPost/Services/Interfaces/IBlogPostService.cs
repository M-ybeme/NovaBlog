using NovaBlog.Models;

namespace NovaBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<bool> ValidateSlugAsync(string title, int blogId);

        public Task AddTagToBlogPostAsync(int tagId, int blogPostId);

        public Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId);

        public Task RemoveTagFromBlogPostAsync(int tagId, int blogPostId);

        public Task<List<Category>> GetCategoriesAsync();

        public Task<List<BlogPost>> GetAllBlogPostsAsync();    // All posts reguardless of is deleted or is published.

        public Task<List<BlogPost>> GetPopularBlogPostsAsync(int count);    // Defined By the number of comments made.

        public Task<List<BlogPost>> GetRecentBlogPostsAsync(int count);    // Defined By the Date created.

    }
}
