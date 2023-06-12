﻿using FluentValidation;
using GameManager.Domain.Entities;

namespace GameManager.Application.Validation;

public class GameValidator : AbstractValidator<Game>
{
    public GameValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(100);
    }    
}