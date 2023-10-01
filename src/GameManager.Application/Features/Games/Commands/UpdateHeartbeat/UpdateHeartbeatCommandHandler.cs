using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.UpdateHeartbeat;

public class UpdateHeartbeatCommandHandler : IRequestHandler<UpdateHeartbeatCommand, UnitResult<CommandError>>
{
    private readonly IPlayerRepository _playerRepository;

    public UpdateHeartbeatCommandHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<UnitResult<CommandError>> Handle(UpdateHeartbeatCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId);

        if (player == null)
        {
            return GameErrors.Commands.PlayerNotFound(request.PlayerId);
        }
        
        player.UpdateHeartbeat();

        await _playerRepository.UpdateAsync(player);

        return UnitResult.Success<CommandError>();
    }
}