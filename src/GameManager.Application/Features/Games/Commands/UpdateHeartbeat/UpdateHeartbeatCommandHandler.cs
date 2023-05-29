using GameManager.Application.Data;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommandHandler : IRequestHandler<UpdateHeartbeatCommand>
{
    private readonly IPlayerRepository _playerRepository;

    public UpdateHeartbeatCommandHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task Handle(UpdateHeartbeatCommand request, CancellationToken cancellationToken)
    {
        await _playerRepository.UpdatePlayerHeartbeat(request.PlayerId);
    }
}