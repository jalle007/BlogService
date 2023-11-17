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
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        { 
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsTokens()
        {
            // Arrange
            var userLogin = new UserLoginDto
            {
                Username = "user",
                Password = "password"
            };
            var content = JsonContent.Create(userLogin);

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
         
        

    }
}