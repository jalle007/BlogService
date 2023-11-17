using BlogService.Database.Entities;

namespace BlogService.Service.Implementations
{
    public class CommandService : ICommandService
    {
        private readonly BlogContext _context;

        public CommandService(BlogContext context)
        {
            _context = context;
        }

        public async Task<BlogPost?> CreateBlogPostAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }
        
        public async Task<Comment?> AddCommentToBlogPostAsync(string blogPostId, Comment comment)
        {
            var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
            if (blogPost == null) return null;
            
            comment.BlogPostId = blogPostId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}
