using BlogService.Database.Entities;
using BlogService.Database.Entities.DTO;
using BlogService.Service.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogService.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostsController : ControllerBase
    {
        private readonly ICommandService _commandService;
        private readonly IQueryService _queryService;

        public BlogPostsController(ICommandService commandService, IQueryService queryService)
        {
            _commandService = commandService;
            _queryService = queryService;
        }
        
        [HttpPost]
        public async Task<ActionResult<BlogPost>> CreateBlogPost([FromBody] BlogPostDTO blogPostDTO)
        {
            var blogPost = new BlogPost
            {
                Author = blogPostDTO.Author,
                Title = blogPostDTO.Title,
                Content = blogPostDTO.Content
            };

            var createdBlogPost = await _commandService.CreateBlogPostAsync(blogPost);
            return CreatedAtAction(nameof(GetBlogPostById), new { id = createdBlogPost?.Id }, createdBlogPost);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPostById(string id)
        {
            var blogPost = await _queryService.GetBlogPostByIdAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            return blogPost;
        }

       
        [HttpPost("{blogPostId}/comments")]
        public async Task<ActionResult<Comment>> AddCommentToBlogPost(string blogPostId, [FromBody] CommentDTO commentDTO)
        {
            var comment = new Comment
            {
                BlogPostId = blogPostId,
                Author = commentDTO.Author,
                Text = commentDTO.Text
            };

            var addedComment = await _commandService.AddCommentToBlogPostAsync(blogPostId, comment);
            if (addedComment == null)
            {
                return NotFound($"BlogPost with ID {blogPostId} not found.");
            }

            return CreatedAtAction(nameof(GetBlogPostById), new { id = blogPostId }, addedComment);
        }

    }
}
