﻿namespace GameManager.Application.Features.Games.DTO;

public class PlayerSummaryDTO
{
    public Guid Id { get; set; }
    
    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public ICollection<TurnDTO> Turns { get; set; } = new List<TurnDTO>();
    
    public ICollection<TrackerHistoryDTO> TrackerHistory { get; set; } = new List<TrackerHistoryDTO>();
}