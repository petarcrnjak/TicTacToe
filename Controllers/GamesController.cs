using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Contracts;
using TicTacToe.Contracts.Dtos;
using TicTacToe.Contracts.Requests;
using TicTacToe.Enums;
using TicTacToe.Services;

namespace TicTacToe.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gamesService;

    public GamesController(IGameService gamesService)
    {
        _gamesService = gamesService;
    }

    /// <summary>
    /// Get paginated list of games.
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of games sorted by newest first. Each item includes players, creation time, status (Open, InProgress, Finished) and winner if available.
    /// Pagination defaults: page = 1, pageSize = 10.
    /// </remarks>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>200 OK with a list of <see cref="GameViewDto"/>.</returns>
    /// <response code="200">List of games returned.</response>
    /// <response code="401">Unauthorized - valid JWT required.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GameViewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var games = await _gamesService.GetGamesAsync(page, pageSize);
        return Ok(games);
    }

    /// <summary>
    /// Search and filter games.
    /// </summary>
    /// <remarks>
    /// Filter games by user (either player), start time range, and/or game status. All filter fields are optional and can be combined in any combination.
    /// Results are paginated and sorted newest first.
    /// </remarks>
    /// <param name="userId">Optional user id to match either player slot.</param>
    /// <param name="startedFromUtc">Optional start time (inclusive) - return games started at or after this UTC time.</param>
    /// <param name="startedToUtc">Optional start time (inclusive) - return games started at or before this UTC time.</param>
    /// <param name="status">Optional game status filter.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>200 OK with a list of <see cref="GameFilterDto"/> matching the filter.</returns>
    /// <response code="200">Filtered list returned.</response>
    /// <response code="401">Unauthorized - valid JWT required.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<GameFilterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Search(
        [FromQuery] string? userId,
        [FromQuery] DateTime? startedFromUtc,
        [FromQuery] DateTime? startedToUtc,
        [FromQuery] GameStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new GamesFilter
        {
            UserId = userId,
            StartedFromUtc = startedFromUtc,
            StartedToUtc = startedToUtc,
            Status = status
        };

        var games = await _gamesService.GetGamesFilterAsync(filter, page, pageSize);
        return Ok(games);
    }

    /// <summary>
    /// Create (open) a new game.
    /// </summary>
    /// <remarks>
    /// The current authenticated user opens a new game and waits for an opponent to join. Returns the created game id in the Location header.
    /// </remarks>
    /// <returns>201 Created with Location header pointing to the new game's board status.</returns>
    /// <response code="201">New game created.</response>
    /// <response code="401">Unauthorized - valid JWT required.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create()
    {
        var gameId = await _gamesService.StartGameAsync();
        // use CreatedAtAction to include a Location header to the board status endpoint
        return CreatedAtAction(nameof(GameStatus), new { gameId }, null);
    }

    /// <summary>
    /// Get board status for a specific game.
    /// </summary>
    /// <remarks>
    /// Returns the board representation and the player whose turn is next (if any).
    /// </remarks>
    /// <param name="gameId">Game id.</param>
    /// <returns>200 OK with <see cref="GameFieldDto"/>.</returns>
    /// <response code="200">Board status returned.</response>
    /// <response code="401">Unauthorized - valid JWT required.</response>
    /// <response code="404">Game not found.</response>
    [HttpGet("{gameId:int}/board")]
    [ProducesResponseType(typeof(GameFieldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GameStatus([FromRoute] int gameId)
    {
        var status = await _gamesService.GetGameBoardByIdAsync(gameId);
        return Ok(status);
    }

    /// <summary>
    /// Join an open game as the opponent.
    /// </summary>
    /// <remarks>
    /// The current authenticated user joins an open game. Returns the updated game status after joining.
    /// </remarks>
    /// <param name="gameId">Game id to join.</param>
    /// <returns>200 OK with <see cref="GameStatusDto"/>.</returns>
    /// <response code="200">Successfully joined; returns updated game status.</response>
    /// <response code="400">Bad request or invalid operation (e.g. joining your own game).</response>
    /// <response code="401">Unauthorized - valid JWT required.</response>
    /// <response code="404">Game not found or not joinable.</response>
    [HttpPost("{gameId:int}/join")]
    [ProducesResponseType(typeof(GameStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JoinGame([FromRoute] int gameId)
    {
        var result = await _gamesService.JoinGameAsync(gameId);
        return Ok(result);
    }

    /// <summary>
    /// Play a move in a game.
    /// </summary>
    /// <remarks>
    /// Move to play (row and column indices, 0-based).
    /// 
    /// The authenticated user plays a move in a game they participate in. Request body contains row and column indices (0-based).
    /// The board is a 3x3 grid (9 fields) arranged in row-major order. Valid row and column values are 0..2.
    /// Field index (0-based) is computed as: index = row * 3 + col.
    /// Example: row = 1 and col = 2 -> index = 1 * 3 + 2 = 5 (0-based) => human-friendly field number 6 (1-based).
    ///
    /// Returns the updated game status after the move (including winner if the move finished the game).
    /// </remarks>
    /// <param name="gameId">Game id.</param>
    /// <param name="move">Move to play: specify row and column indices (0-based). See remarks for mapping to the board index.</param>
    /// <returns>200 OK with <see cref="GameStatusDto"/>.</returns>
    /// <response code="200">Move accepted; returns updated game status.</response>
    /// <response code="400">Invalid move or business rule violation.</response>
    /// <response code="401">Unauthorized - valid JWT required.</response>
    /// <response code="404">Game or board not found.</response>
    [HttpPost("{gameId:int}/play")]
    [ProducesResponseType(typeof(GameStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PlayGame([FromRoute] int gameId, [FromBody] GameMove move)
    {
        var result = await _gamesService.PlayGameAsync(gameId, move);
        return Ok(result);
    }
}
