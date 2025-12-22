using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Contracts;
using TicTacToe.Contracts.Requests;
using TicTacToe.Enums;
using TicTacToe.Services;

namespace TicTacToe.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly IGameService _gamesService;

    public GamesController(IGameService gamesService)
    {
        _gamesService = gamesService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var games = await _gamesService.GetGamesAsync(page, pageSize);
        return Ok(games);
    }


    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? userId, [FromQuery] DateTime? startedFromUtc, [FromQuery] DateTime? startedToUtc, [FromQuery] GameStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var gameId = await _gamesService.StartGameAsync();
        return Created();
    }

    [HttpGet("{gameId:int}/board")]
    public async Task<IActionResult> GameStatus([FromRoute] int gameId)
    {
        var status = await _gamesService.GetGameBoardByIdAsync(gameId);
        return Ok(status);
    }

    [HttpPost("{gameId:int}/join")]
    public async Task<IActionResult> JoinGame([FromRoute] int gameId)
    {
        var result = await _gamesService.JoinGameAsync(gameId);
        return Ok(result);
    }

    [HttpPost("{gameId:int}/play")]
    public async Task<IActionResult> PlayGame([FromRoute] int gameId, [FromBody] GameMove move)
    {
        var result = await _gamesService.PlayGameAsync(gameId, move);
        return Ok(result);
    }
}
