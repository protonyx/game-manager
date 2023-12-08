using FluentValidation.Results;
using GameManager.Application.Errors;
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
        public const string PlayerNotAdmin = "player.not_admin";
    }
    
    public static ApplicationError InvalidEntryCode()
        => ApplicationError.Failure("Invalid entry code",
            errorCode: ErrorCodes.GameInvalidEntryCode);
        
    public static ApplicationError GameNotFound(Guid? gameId)
        => ApplicationError.NotFound<Game>(gameId?.ToString(),
            errorCode: ErrorCodes.GameInvalidId);
        
    public static ApplicationError GameNotInProgress(Guid gameId)
        => ApplicationError.Failure("Game not in progress",
            errorCode: ErrorCodes.GameInvalidState);

    public static ApplicationError GameAlreadyInProgress() =>
        ApplicationError.Failure("Game already in progress",
            errorCode: ErrorCodes.GameInvalidState);

    public static ApplicationError GameNotComplete() =>
        ApplicationError.Failure("Game not complete",
            errorCode: ErrorCodes.GameInvalidState);

    public static ApplicationError PlayerNotFound(Guid? playerId)
        => ApplicationError.NotFound<Player>(playerId?.ToString(),
            errorCode: ErrorCodes.PlayerInvalidId);

    public static ApplicationError PlayerNotAuthorized(string? action = null) =>
        ApplicationError.Authorization(string.IsNullOrWhiteSpace(action)
                ? "Player is not authorized to perform this action"
                : $"Player is not authorized to {action}",
            errorCode: ErrorCodes.PlayerNotAuthorized);

    public static ApplicationError PlayerNotAdmin() =>
        ApplicationError.Authorization("Player is not an admin for this game",
            errorCode: ErrorCodes.PlayerNotAdmin);

    public static ApplicationError PlayerNotInGame() =>
        ApplicationError.Authorization("Player is not in the game",
            errorCode: ErrorCodes.PlayerNotAuthorized);

    public static ApplicationError PlayerInvalidName(string? reason = null) =>
        ApplicationError.Validation(
            string.IsNullOrWhiteSpace(reason)
                ? "Player name is invalid"
                : $"Player name is invalid: {reason}",
            errorCode: ErrorCodes.PlayerInvalidName);
}