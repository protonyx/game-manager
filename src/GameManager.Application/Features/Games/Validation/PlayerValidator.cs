using FluentValidation;
using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;

namespace GameManager.Application.Features.Games.Validation;

public class PlayerValidator : AbstractValidator<Player>
{
    public PlayerValidator(IGameRepository gameRepository, IPlayerRepository playerRepository)
    {
        RuleFor(t => t.GameId)
            .MustAsync(async (gameId, cancellationToken) => await gameRepository.ExistsAsync(gameId, cancellationToken))
            .WithMessage("Game must exist");

        RuleFor(t => t.Name)
            .MustAsync((player, name, cancellationToken) =>
                playerRepository.NameIsUniqueAsync(player.GameId, name, player.Id, cancellationToken))
            .WithMessage("{PropertyName} must be unique");
    }
}