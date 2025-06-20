using FastEndpoints;
using FluentValidation;

namespace GameManager.Server.Endpoints.Games;

public class JoinGameDTOValidator : Validator<JoinGameDTO>
{
    public JoinGameDTOValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .Length(3, 20)
            .When(t => !t.Observer);

        RuleFor(t => t.EntryCode)
            .NotEmpty();
    }
}