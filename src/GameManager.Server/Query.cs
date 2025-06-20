using AutoMapper.QueryableExtensions;
using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Authorization;
using GameManager.Server.DataLoaders;
using GameManager.Server.Filters;
using GameManager.Server.Models;
using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Server;

public class Query
{
    [Authorize(AuthorizationPolicyNames.Admin)]
    [UsePaging(MaxPageSize = 100, IncludeTotalCount = true, AllowBackwardPagination = false)]
    [UseFiltering(typeof(GameFilterType))]
    public async Task<Connection<GameModel>> GetGames(
        IGameRepository gameRepository,
        IMapper mapper,
        IResolverContext resolverContext,
        string? after,
        int? first,
        CancellationToken cancellationToken = default)
    {
        IQueryable<GameModel> query = gameRepository.Query()
            .ProjectTo<GameModel>(mapper.ConfigurationProvider)
            .OrderBy(t => t.Id);

        // Apply filters
        //var filters = resolverContext.GetFilterContext();
        query = query.Filter(resolverContext);

        var totalQuery = query;

        if (!string.IsNullOrWhiteSpace(after))
        {
            var cursor = Guid.Parse(after);
            query = query.Where(t => t.Id > cursor);
        }

        if (first.HasValue)
        {
            query = query.Take(first.Value);
        }

        var games = await query.ToListAsync(cancellationToken);

        var edges = games
            .Select(g => new Edge<GameModel>(g, g.Id.ToString()))
            .ToList();
        var endCursor = edges.Count > 0
            ? games.Max(t => t.Id).ToString()
            : null;

        var pageInfo = new ConnectionPageInfo(endCursor != null, false, null,
            endCursor);

        var totalCount = await totalQuery.CountAsync(cancellationToken);

        return new Connection<GameModel>(edges, pageInfo, totalCount);
    }

    [Authorize(AuthorizationPolicyNames.ViewGame, ApplyPolicy.BeforeResolver)]
    public async Task<GameModel?> GetGameById(
        Guid id,
        GameByIdDataLoader gameById,
        CancellationToken cancellationToken)
    {
        return await gameById.LoadAsync(id, cancellationToken);
    }
}