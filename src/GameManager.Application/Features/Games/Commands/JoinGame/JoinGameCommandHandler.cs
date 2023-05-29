using System.ComponentModel.DataAnnotations;
using AutoMapper;
using GameManager.Application.Data;
using GameManager.Application.Services;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommandHandler : IRequestHandler<JoinGameCommand, JoinGameCommandResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly ITokenService _tokenService;

    private readonly IMapper _mapper;

    public JoinGameCommandHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        ITokenService tokenService,
        IMapper mapper)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<JoinGameCommandResponse> Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var ret = new JoinGameCommandResponse();
        
        var game = await _gameRepository.GetGameByEntryCodeAsync(request.EntryCode);
        
        if (game == null)
        {
            ret.ValidationResults.Add(new ValidationResult(
                "The provided entry code is invalid.",
                new []{nameof(JoinGameCommand.EntryCode)}));

            return ret;
        }
        
        var newPlayer = new Player()
        {
            GameId = game.Id,
            Name = request.Name
        };

        try
        {
            newPlayer = await _playerRepository.CreateAsync(newPlayer);
        
            ret.GameId = game.Id;
            ret.PlayerId = newPlayer.Id;
            ret.Token = _tokenService.GenerateToken(game.Id, newPlayer.Id, newPlayer.IsAdmin);
            ret.IsAdmin = newPlayer.IsAdmin;
        }
        catch (ValidationException e)
        {
            ret.ValidationResults.Add(e.ValidationResult);
        }

        return ret;
    }
}