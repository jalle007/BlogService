using BlogService.Database.Entities;

namespace BlogService.Service.Implementations
{
    public interface ICommandService
    {
        Task<BlogPost?> CreateBlogPostAsync(BlogPost blogPost);
        Task<Comment?> AddCommentToBlogPostAsync(string blogPostId, Comment comment);
    }
}
