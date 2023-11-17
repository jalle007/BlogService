using BlogService.Database.Entities;
using BlogService.Database.Entities.DTO;
using BlogService.Service.Implementations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlogService.Tests
{
    public class BlogPostsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly JwtTokenService _jwtTokenService;
        private readonly HttpClient _client;

        public BlogPostsControllerTests(WebApplicationFactory<Program> factory)
        {
            var mock = new Mock<IConfiguration>();
            mock.Setup(c => c["JwtConfig:Secret"]).Returns("rUp9H3ox0MffJPQ8A8E/J6DNEvjjiM2OAwCriDMm31s=");

            _factory = factory;
            _jwtTokenService = new JwtTokenService(mock.Object);
            _client = SetupClient();
        }

        private HttpClient SetupClient()
        {
            var client = _factory.CreateClient();
            var token = _jwtTokenService.GenerateToken("testuser");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        [Fact]
        public async Task GetBlogPostById_ReturnsNotFoundForInvalidId  ()
        {
            // Arrange
            string invalidId = "invalidId";

            // Act
            var response = await _client.GetAsync($"/api/blogposts/{invalidId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddingCommentToNonExistingBlogPost_ReturnsNotFount() 
        {
            string invalidId = "invalidId";
            var commentDTO =  new CommentDTO
            {
                Author = "testuser",
                Text = "This is a comment"
            };
            var content = JsonContent.Create(commentDTO);

            // Act
            var response = await _client.PostAsync($"/api/blogposts/{invalidId}/comments", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateBlogPost_ReturnsCreatedAction() 
        {
            var blogPost = new BlogPostDTO
            {
                Author = "testuser",
                Title = "Test Blog Post",
                Content = "This is a test blog post"
            };
            var content = JsonContent.Create(blogPost);

            // Act
            var response = await _client.PostAsync("/api/blogposts", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task AddCommentToExistingBlogPost_ReturnsCreatedAction() 

        { 
            var blogPost = new BlogPostDTO
            {
                Author = "testuser",
                Title = "Test Blog Post",
                Content = "This is a test blog post"
            };

            var content = JsonContent.Create(blogPost);
            var response = await _client.PostAsync("/api/blogposts", content);
            var createdBlogPost= (await response.Content.ReadFromJsonAsync<BlogPost>());

            var newComment = new CommentDTO
            {
                Author = "testuser",
                Text = "This is a comment"
            };
            content = JsonContent.Create(newComment);

            // Act
            response = await _client.PostAsync($"/api/blogposts/{createdBlogPost?.Id}/comments", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        //[Fact]
        //public async Task ExceedRateLimit_ReturnsTooManyRequests() 
        //{
        //    // Arrange
        //    var requests = 11; // 10 is the limit
        //    var blogPost = new BlogPostDTO
        //    {
        //        Author = "testuser",
        //        Title = "Test Blog Post",
        //        Content = "This is a test blog post"
        //    };

        //    HttpResponseMessage lastResponse = null;
        //    for (int i = 0; i < requests; i++)
        //    {
        //        var content = JsonContent.Create(blogPost);
        //        lastResponse = await _client.PostAsync("/api/blogposts", content);
        //    }

        //    // Assert
        //    Assert.NotNull(lastResponse);
        //    Assert.Equal(HttpStatusCode.TooManyRequests, lastResponse.StatusCode);
        //}

    }
}