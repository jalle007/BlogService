using BlogService.Database.Entities.DTO;
using BlogService.Service.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace BlogService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] UserLoginDto userLogin)
        {
            // Validate user credentials (this is simplified for demonstration)
            if (userLogin.Username == "user" && userLogin.Password == "password")
            {
                var token = _jwtTokenService.GenerateToken(userLogin.Username);
                return Ok(token);
            }

            return Unauthorized();
        }
    }

}
