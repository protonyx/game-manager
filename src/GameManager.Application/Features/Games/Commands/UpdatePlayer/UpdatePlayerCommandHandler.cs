using AutoMapper;
using FluentValidation;
using GameManager.Application.Commands;
using GameManager.Application.Data;
using GameManager.Application.DTO;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, ICommandResponse>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    private readonly IValidator<Player> _playerValidator;

    public UpdatePlayerCommandHandler(IPlayerRepository playerRepository, IMapper mapper, IValidator<Player> playerValidator)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
        _playerValidator = playerValidator;
    }

    public async Task<ICommandResponse> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId);

        if (player == null)
        {
            return CommandResponses.NotFound();
        }

        // Map changes onto entity
        _mapper.Map(request.Player, player);
        
        // Validate
        var result = await _playerValidator.ValidateAsync(player, cancellationToken);

        if (!result.IsValid)
        {
            return CommandResponses.ValidationError(result);
        }

        await _playerRepository.UpdateAsync(player);

        var dto = _mapper.Map<PlayerDTO>(player);

        return CommandResponses.Data(request.PlayerId, dto);
    }
}