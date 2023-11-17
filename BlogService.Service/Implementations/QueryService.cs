using BlogService.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BlogService.Service.Implementations
{
    public class QueryService : IQueryService
    {
        private readonly BlogContext _context;
        private readonly IDistributedCache _cache;

        public QueryService(BlogContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(string blogPostId)
        {
            var cacheKey = $"BlogPost-{blogPostId}";
            BlogPost? blogPost ;

            var cachedBlogPost = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedBlogPost))
            { 
                blogPost = JsonSerializer.Deserialize<BlogPost>(cachedBlogPost);
                return blogPost;
            }
            else
            {
                blogPost = await _context.BlogPosts
                    .Include(bp => bp.Comments)
                    .FirstOrDefaultAsync(bp => bp.Id == blogPostId);

                if (blogPost != null)
                {
                    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(blogPost), new DistributedCacheEntryOptions 
                    { 
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }

            return blogPost;
        }

        public async Task<List<Comment>> GetCommentsByBlogPostIdAsync(string blogPostId)
        {
            var cacheKey = $"Comments-{blogPostId}";
            List<Comment> comments;

            var cachedComments = await _cache.GetStringAsync(cacheKey);
            if(!string.IsNullOrEmpty(cachedComments))
            {
                comments = JsonSerializer.Deserialize<List<Comment>>(cachedComments) ?? new List<Comment>();
            }
            else
            {
                comments = await _context.Comments
                    .Where(c => c.BlogPostId == blogPostId)
                    .ToListAsync();

                if (comments != null)
                {
                    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(comments), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }

            return comments ?? new List<Comment>();
        }
    }
}
