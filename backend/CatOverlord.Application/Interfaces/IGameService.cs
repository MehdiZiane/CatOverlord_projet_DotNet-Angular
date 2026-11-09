using CatOverlord.Application.DTOs;

namespace CatOverlord.Application.Interfaces;

public interface IGameService
{
    Task<GameStateDto> GetStateAsync();
    Task<GameStateDto> ClickAsync();
}