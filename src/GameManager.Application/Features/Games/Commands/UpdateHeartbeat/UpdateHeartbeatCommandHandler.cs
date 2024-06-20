using GameManager.Application.Contracts;
using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommandHandler : ICommandHandler<UpdateHeartbeatCommand>
{
    private readonly IPlayerRepository _playerRepository;

    public UpdateHeartbeatCommandHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<UnitResult<ApplicationError>> Handle(UpdateHeartbeatCommand request, CancellationToken cancellationToken)
    {
        await _playerRepository.UpdateHeartbeatAsync(request.PlayerId, request.ConnectionId, cancellationToken);

        return UnitResult.Success<ApplicationError>();
    }
}