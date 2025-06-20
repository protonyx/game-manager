using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommandValidator : AbstractValidator<JoinGameCommand>
{
    public JoinGameCommandValidator(IGameRepository gameRepository)
    {
        RuleFor(t => t.EntryCode)
            .Must(t => EntryCode.From(t).IsSuccess)
            .MustAsync((t, ct) => gameRepository.EntryCodeExistsAsync(EntryCode.From(t).Value, ct))
            .WithMessage("{PropertyName} is not valid.");
    }
}