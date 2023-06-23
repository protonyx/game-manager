using GameManager.Application.Commands;
using GameManager.Application.Data;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommandHandler : IRequestHandler<UpdateHeartbeatCommand, ICommandResponse>
{
    private readonly IPlayerRepository _playerRepository;

    public UpdateHeartbeatCommandHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<ICommandResponse> Handle(UpdateHeartbeatCommand request, CancellationToken cancellationToken)
    {
        await _playerRepository.UpdatePlayerHeartbeatAsync(request.PlayerId);
        
        return CommandResponses.Success();
    }
}