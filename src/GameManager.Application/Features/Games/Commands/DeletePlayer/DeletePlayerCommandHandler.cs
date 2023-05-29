using GameManager.Application.Data;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand>
{
    private readonly IPlayerRepository _playerRepository;

    public DeletePlayerCommandHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
    {
        return _playerRepository.DeleteByIdAsync(request.PlayerId);
    }
}