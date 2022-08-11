namespace NovaBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<bool> ValidateSlugAsync(string title, int blogId);

        public Task AddTagToBlogPostAsync(int tagId, int blogPostId);

        public Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId);

    }
}
