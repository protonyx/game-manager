using AutoMapper;
using GameManager.Server.Data;
using GameManager.Server.DTO;
using GameManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly PlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    public PlayersController(PlayerRepository playerRepository, IMapper mapper)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
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

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer([FromRoute] Guid id)
    {
        await _playerRepository.RemovePlayer(id);

        return NoContent();
    }
}