using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Contracts.Dtos;
using TicTacToe.Services;

namespace TicTacToe.Controllers;

/// <summary>
/// User endpoints for the TicTacToe API.
/// </summary>
/// <remarks>
/// Returns a user's public profile containing:
/// - userName: user's display name
/// - wins: total wins
/// - gamesPlayed: total games played
/// - winPercentage: percentage of wins (0.0 - 100.0), computed as (wins * 100.0) / gamesPlayed and rounded to two decimals. If gamesPlayed == 0 the value is 0.0.
/// 
/// Example response:
/// {
///   "userName": "player1",
///   "wins": 10,
///   "gamesPlayed": 25,
///   "winPercentage": 40.00
/// }
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _usersService;

    public UsersController(IUserService usersService)
    {
        _usersService = usersService;
    }

    /// <summary>
    /// Get user profile by id.
    /// </summary>
    /// <param name="id">User id to look up.</param>
    /// <returns>200 OK with <see cref="UserDto"/> on success.</returns>
    /// <response code="200">Profile found and returned.</response>
    /// <response code="401">Unauthorized - valid JWT required.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Profile(string id)
    {
        var user = await _usersService.GetUserByIdAsync(id);
        return Ok(user);
    }
}
