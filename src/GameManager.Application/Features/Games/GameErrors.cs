using FluentValidation.Results;
using GameManager.Application.Contracts.Commands;
using GameManager.Domain.Entities;

namespace GameManager.Application.Features.Games;

public static class GameErrors
{
    public static class ErrorCodes
    {
        public const string GameInvalidEntryCode = "game.invalid.code";
        public const string GameInvalidId = "game.invalid.id";
        public const string GameInvalidState = "game.invalid.state";
        public const string PlayerInvalidId = "player.invalid.id";
        public const string PlayerInvalidState = "player.invalid.state";
        public const string PlayerInvalidName = "player.invalid.name";
        public const string PlayerNotAuthorized = "player.not_authorized";
    }
    
    public static class Commands
    {
        public static CommandError InvalidEntryCode()
            => CommandError.Failure("Invalid entry code",
                errorCode: ErrorCodes.GameInvalidEntryCode);
        
        public static CommandError GameNotFound(Guid? gameId)
            => CommandError.NotFound<Game>(gameId?.ToString(),
                errorCode: ErrorCodes.GameInvalidId);
        
        public static CommandError GameNotInProgress(Guid gameId)
            => CommandError.Failure("Game not in progress",
                errorCode: ErrorCodes.GameInvalidState);

        public static CommandError GameAlreadyInProgress() =>
            CommandError.Failure("Game already in progress",
                errorCode: ErrorCodes.GameInvalidState);

        public static CommandError PlayerNotFound(Guid? playerId)
            => CommandError.NotFound<Player>(playerId?.ToString(),
                errorCode: ErrorCodes.PlayerInvalidId);

        public static CommandError PlayerNotAuthorized(string? action = null) =>
            CommandError.Authorization(string.IsNullOrWhiteSpace(action)
                    ? "Player is not authorized to perform this action"
                    : $"Player is not authorized to {action}",
                errorCode: ErrorCodes.PlayerNotAuthorized);

        public static CommandError PlayerNotInGame() =>
            CommandError.Authorization("Player is not in the game",
                errorCode: ErrorCodes.PlayerNotAuthorized);

        public static CommandError PlayerInvalidName(string? reason = null) =>
            CommandError.Validation(
                string.IsNullOrWhiteSpace(reason)
                    ? "Player name is invalid"
                    : $"Player name is invalid: {reason}",
                errorCode: ErrorCodes.PlayerInvalidName);
    }
}