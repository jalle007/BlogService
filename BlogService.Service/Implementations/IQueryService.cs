using BlogService.Database.Entities;

namespace BlogService.Service.Implementations
{
    public interface IQueryService
    {
        Task<BlogPost?> GetBlogPostByIdAsync(string blogPostId);
        Task<List<Comment>> GetCommentsByBlogPostIdAsync(string blogPostId);
    }

}
