using AutoMapper;
using GameManager.Application.Data;
using GameManager.Application.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, UpdatePlayerCommandResponse>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    public UpdatePlayerCommandHandler(IPlayerRepository playerRepository, IMapper mapper)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    public async Task<UpdatePlayerCommandResponse> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        var ret = new UpdatePlayerCommandResponse();

        var player = await _playerRepository.GetByIdAsync(request.PlayerId);

        if (player == null)
        {
            return ret;
        }

        // Map changes onto entity
        _mapper.Map(request.Player, player);
        
        // TODO: Validate

        await _playerRepository.UpdateAsync(player);

        ret.Player = _mapper.Map<PlayerDTO>(player);

        return ret;
    }
}