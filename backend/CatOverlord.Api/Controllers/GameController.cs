using CatOverlord.Application.DTOs;
using CatOverlord.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatOverlord.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService) => _gameService = gameService;

    [HttpGet("state")]
    public async Task<ActionResult<GameStateDto>> GetState()
        => Ok(await _gameService.GetStateAsync());

    [HttpPost("click")]
    public async Task<ActionResult<GameStateDto>> Click()
        => Ok(await _gameService.ClickAsync());
}