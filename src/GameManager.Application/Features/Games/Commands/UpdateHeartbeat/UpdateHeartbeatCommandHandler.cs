using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommandHandler : IRequestHandler<UpdateHeartbeatCommand, UnitResult<ApplicationError>>
{
    private readonly IPlayerRepository _playerRepository;

    public UpdateHeartbeatCommandHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<UnitResult<ApplicationError>> Handle(UpdateHeartbeatCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId, cancellationToken);

        if (player == null)
        {
            return GameErrors.PlayerNotFound(request.PlayerId);
        }
        
        player.UpdateHeartbeat();

        await _playerRepository.UpdateAsync(player, cancellationToken);

        return UnitResult.Success<ApplicationError>();
    }
}