namespace GameManager.Application.Contracts;

public interface IPlayerIdentityBuilder
{
    IPlayerIdentityBuilder AddGameId(Guid gameId);
    IPlayerIdentityBuilder AddPlayerId(Guid playerId);
    IPlayerIdentityBuilder AddAdminRole();
}