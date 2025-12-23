using Microsoft.AspNetCore.Mvc;
using TicTacToe.Contracts.Requests.Auth;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    /// <summary>
    /// Authentication endpoints for the TicTacToe API.
    /// </summary>
    /// <remarks>
    /// Provides endpoints to register a new user and to obtain an authentication token via login.
    /// XML comments are used so Swagger (Swashbuckle) can include summaries, remarks and request/response information.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <remarks>
        /// Creates a new user account using the supplied username and password.
        ///
        /// Required request body properties:
        /// - <c>username</c> (string): the desired username. Required.
        /// - <c>password</c> (string): the account password. Required.
        ///
        /// Example request:
        /// {
        ///   "username": "player1",
        ///   "password": "P@ssw0rd"
        /// }
        /// </remarks>
        /// <param name="req">Request body containing the required <c>username</c> and <c>password</c>.</param>
        /// <returns>200 OK with the created username on success.</returns>
        /// <response code="200">Registration successful. Returns the registered username.</response>
        /// <response code="400">Bad request - request payload is invalid.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var username = await _auth.RegisterAsync(req);
            return Ok(new { username });
        }

        /// <summary>
        /// Authenticate a user and obtain a JWT token.
        /// </summary>
        /// <remarks>
        /// Validates credentials and returns a bearer token to be used for authenticated requests.
        ///
        /// Required request body properties:
        /// - <c>username</c> (string): account username. Required.
        /// - <c>password</c> (string): account password. Required.
        ///
        /// Example request:
        /// {
        ///   "username": "player1",
        ///   "password": "P@ssw0rd"
        /// }
        /// </remarks>
        /// <param name="req">Request body containing the required <c>username</c> and <c>password</c>.</param>
        /// <returns>200 OK with authentication token and username on success.</returns>
        /// <response code="200">Login successful. Returns a token and the username.</response>
        /// <response code="400">Bad request - request payload is invalid.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var response = await _auth.LoginAsync(req);
            return Ok(new { response.Token, response.Username });
        }
    }
}
