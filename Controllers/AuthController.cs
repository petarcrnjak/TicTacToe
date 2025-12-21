using Microsoft.AspNetCore.Mvc;
using TicTacToe.Contracts.Requests.Auth;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var username = await _auth.RegisterAsync(req);
            return Ok(new { username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var response = await _auth.LoginAsync(req);
            return Ok(new { response.Token, response.Username });
        }
    }
}
