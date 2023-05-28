using AutoMapper;
using GameManager.Server.Data;
using GameManager.Server.DTO;
using GameManager.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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

    public PlayersController(
        PlayerRepository playerRepository,
        IMapper mapper)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayer([FromRoute] Guid id)
    {
        var player = await _playerRepository.GetByIdAsync(id);

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
        var playerUpdates = _mapper.Map<Player>(dto);

        var player = await _playerRepository.UpdateAsync(playerUpdates);

        if (player == null)
        {
            return NotFound();
        }
        
        dto = _mapper.Map<PlayerDTO>(player);

        return Ok(dto);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchPlayer(
        [FromRoute] Guid id,
        [FromBody] JsonPatchDocument<PlayerDTO> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest(ModelState);
        }

        var player = await _playerRepository.GetByIdAsync(id);

        if (player == null)
        {
            return NotFound();
        }

        var dto = _mapper.Map<PlayerDTO>(player);
        
        patchDoc.ApplyTo(dto, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(dto, player);

        await _playerRepository.UpdateAsync(player);

        _mapper.Map(player, dto);

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer([FromRoute] Guid id)
    {
        var player = await _playerRepository.GetByIdAsync(id);

        if (player is not {Active: true})
        {
            return NotFound();
        }
        
        await _playerRepository.DeleteAsync(player);

        return NoContent();
    }
}