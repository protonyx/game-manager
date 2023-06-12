using FluentValidation;
using GameManager.Application.Data;
using GameManager.Domain.Entities;

namespace GameManager.Application.Validation;

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
            .NotNull()
            .Length(3, 20);

        RuleFor(t => t.Name)
            .MustAsync((player, name, cancellationToken) =>
                playerRepository.NameIsUnique(player.GameId, name))
            .WithMessage("{PropertyName} must be unique");
    }
}