using BlogService.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlogService.Service.Implementations
{
    public class QueryService : IQueryService
    {
        private readonly BlogContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<QueryService> _logger;

        public QueryService(BlogContext context, IDistributedCache cache, ILogger<QueryService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        ///  Get a blog post by its ID. 
        ///  If Redis cache is not available, blog post will be retrieved from the database and returned without caching.
        ///  All exceptions will be logged in.
        /// </summary>
        /// <param name="blogPostId"></param>
        /// <returns></returns>
        public async Task<BlogPost?> GetBlogPostByIdAsync(string blogPostId)
        {
            var cacheKey = $"BlogPost-{blogPostId}";
            BlogPost? blogPost = null;
            
            try
            {
                var cachedBlogPost = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedBlogPost))
                {
                    blogPost = JsonSerializer.Deserialize<BlogPost>(cachedBlogPost);
                    //_logger.LogInformation("Retrieved blog post with ID {BlogPostId} from cache", blogPostId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve blog post with ID {BlogPostId} from cache", blogPostId);
            }

            if (blogPost == null)
            {
                blogPost = await _context.BlogPosts
                    .Include(bp => bp.Comments)
                    .FirstOrDefaultAsync(bp => bp.Id == blogPostId);

                if (blogPost != null)
                {
                    try
                    {
                        var serializedBlogPost = JsonSerializer.Serialize(blogPost);
                        await _cache.SetStringAsync(cacheKey, serializedBlogPost, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });
                        //_logger.LogInformation("Cached blog post with ID {BlogPostId}", blogPostId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to cache blog post with ID {BlogPostId}", blogPostId);
                    }
                }
                else
                {
                    _logger.LogWarning("Blog post with ID {BlogPostId} not found in database", blogPostId);
                }
            }

            return blogPost;
        }

        /// <summary>
        ///  Get a list of comments for blog post by its ID. 
        ///  If Redis cache is not available, comments will be retrieved from the database and returned without caching.
        ///  All exceptions will be logged in.
        /// </summary>
        /// <param name="blogPostId"></param>
        /// <returns></returns>
        public async Task<List<Comment>> GetCommentsByBlogPostIdAsync(string blogPostId)
        {
            var cacheKey = $"Comments-{blogPostId}";
            List<Comment> comments = new List<Comment>();

            try
            {
                var cachedComments = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedComments))
                {
                    comments = JsonSerializer.Deserialize<List<Comment>>(cachedComments) ?? new List<Comment>();
                    //_logger.LogInformation("Retrieved comments for blog post ID {BlogPostId} from cache", blogPostId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve comments for blog post ID {BlogPostId} from cache", blogPostId);
            }

            if (comments.Count == 0)
            {
                comments = await _context.Comments
                    .Where(c => c.BlogPostId == blogPostId)
                    .ToListAsync();

                if (comments.Any())
                {
                    try
                    {
                        var serializedComments = JsonSerializer.Serialize(comments);
                        await _cache.SetStringAsync(cacheKey, serializedComments, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });
                        //_logger.LogInformation("Cached comments for blog post ID {BlogPostId}", blogPostId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to cache comments for blog post ID {BlogPostId}", blogPostId);
                    }
                }
                else
                {
                    _logger.LogWarning("No comments found for blog post ID {BlogPostId} in database", blogPostId);
                }
            }

            return comments;
        }

    }
}
