using FluentValidation;
using GameManager.Application.Contracts.Persistence;
using GameManager.Domain.Entities;

namespace GameManager.Application.Features.Games.Validation;

public class PlayerValidator : AbstractValidator<Player>
{
    public PlayerValidator(IGameRepository gameRepository, IPlayerRepository playerRepository)
    {
        RuleFor(t => t.GameId)
            .MustAsync(async (gameId, cancellationToken) =>
            {
                var game = await gameRepository.GetByIdAsync(gameId);

                return game != null;
            })
            .WithMessage("Game must exist");

        RuleFor(t => t.Name)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull();
            //.Length(3, 20);

        RuleFor(t => t.Name)
            .MustAsync((player, name, cancellationToken) =>
                playerRepository.NameIsUniqueAsync(player.GameId, name.Value, player.Id))
            .WithMessage("{PropertyName} must be unique");
    }
}