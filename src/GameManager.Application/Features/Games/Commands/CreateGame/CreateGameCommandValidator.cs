using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(GameName.MaximumLength);

        RuleForEach(t => t.Trackers)
            .SetValidator(new CreateTrackerValidator());
    }
}

public class CreateTrackerValidator : AbstractValidator<CreateTrackerDTO>
{
    public CreateTrackerValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(20);
    }
}