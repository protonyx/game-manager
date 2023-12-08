using AutoMapper;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetPlayer;

public class GetPlayerQueryHandler : IRequestHandler<GetPlayerQuery, Result<PlayerDTO, ApplicationError>>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    public GetPlayerQueryHandler(IPlayerRepository playerRepository, IMapper mapper)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    public async Task<Result<PlayerDTO, ApplicationError>> Handle(GetPlayerQuery request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId, cancellationToken);

        if (player == null)
        {
            return GameErrors.PlayerNotFound(request.PlayerId);
        }

        var dto = _mapper.Map<PlayerDTO>(player);

        return dto;
    }
}