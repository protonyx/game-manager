using GameManager.Application.Commands;
using GameManager.Application.Data;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, ICommandResponse>
{
    private readonly IPlayerRepository _playerRepository;

    public DeletePlayerCommandHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<ICommandResponse> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
    {
        await _playerRepository.DeleteByIdAsync(request.PlayerId);

        return CommandResponses.Success();
    }
}