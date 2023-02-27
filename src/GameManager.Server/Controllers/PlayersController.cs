using AutoMapper;
using GameManager.Server.Data;
using GameManager.Server.DTO;
using GameManager.Server.Messages;
using GameManager.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PlayersController : ControllerBase
{
    private readonly PlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    private readonly IHubContext<GameHub> _hubContext;

    public PlayersController(
        PlayerRepository playerRepository,
        IMapper mapper,
        IHubContext<GameHub> hubContext)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayer([FromRoute] Guid id)
    {
        var player = await _playerRepository.GetPlayerById(id);

        if (player == null)
        {
            return NotFound();
        }

        var dto = _mapper.Map<PlayerDTO>(player);

        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(
        [FromRoute] Guid id,
        [FromBody] PlayerDTO dto)
    {
        var playerUpdates = new Player()
        {
            Order = dto.Order,
            Name = dto.Name,
            TrackerValues = dto.TrackerValues.Select(kv => new TrackerValue()
            {
                TrackerId = kv.Key,
                Value = kv.Value
            }).ToList()
        };

        var player = await _playerRepository.UpdatePlayerAsync(id, playerUpdates);

        if (player == null)
        {
            return NotFound();
        }
        
        dto = _mapper.Map<PlayerDTO>(player);
        
        // Notify other players
        var message = new PlayerStateChangedMessage()
        {
            GameId = player.GameId,
            Player = dto
        };

        await _hubContext.Clients.Group(player.GameId.ToString())
            .SendAsync(nameof(IGameHubClient.PlayerStateChanged), message);

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer([FromRoute] Guid id)
    {
        var player = await _playerRepository.GetPlayerById(id);

        if (player == null)
        {
            return NotFound();
        }
        
        await _playerRepository.RemovePlayer(id);
        
        // Notify other players
        var message = new PlayerLeftMessage()
        {
            GameId = player.GameId,
            PlayerId = player.Id
        };

        await _hubContext.Clients.Group(player.GameId.ToString())
            .SendAsync(nameof(IGameHubClient.PlayerLeft), message);

        return NoContent();
    }
}