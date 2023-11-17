using BlogService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogService.Service.Implementations
{
    public class QueryService : IQueryService
    {
        private readonly BlogContext _context;

        public QueryService(BlogContext context)
        {
            _context = context;
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(string blogPostId)
        {
            return await _context.BlogPosts
                .Include(bp => bp.Comments)
                .FirstOrDefaultAsync(bp => bp.Id == blogPostId);
        }

        public async Task<List<Comment>> GetCommentsByBlogPostIdAsync(string blogPostId)
        {
            return await _context.Comments
                .Where(c => c.BlogPostId == blogPostId)
                .ToListAsync();
        }
    }
}
