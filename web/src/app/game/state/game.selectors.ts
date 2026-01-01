import { createSelector } from '@ngrx/store';
import { playerAdapter } from './game.state';
import {
  PlayerSummary,
  PlayerTrackerHistory,
  TrackerSummary,
} from '../models/models';
import { gameFeature } from './game.reducer';

export const { selectCredentials, selectGame, selectPlayers, selectSummary } =
  gameFeature;

export const selectGameTrackers = createSelector(
  selectGame,
  (game) => game?.trackers,
);

const { selectEntities, selectAll } = playerAdapter.getSelectors();

export const selectPlayersEntities = createSelector(
  selectPlayers,
  selectEntities,
);

export const selectAllPlayers = createSelector(selectPlayers, selectAll);

export const selectPlayerById = (playerId: string) =>
  createSelector(
    selectPlayersEntities,
    (playerEntities) => (playerId ? playerEntities[playerId] : null),
  );

export const selectCurrentPlayerId = createSelector(
  selectCredentials,
  (credentials) => credentials?.playerId,
);

export const selectCurrentPlayerIsHost = createSelector(
  selectCredentials,
  (credentials) => credentials?.isHost || false,
);

export const selectCurrentPlayer = createSelector(
  selectCurrentPlayerId,
  selectPlayersEntities,
  (playerId, entities) => (playerId ? entities[playerId] : undefined),
);

export const selectSummaryName = createSelector(selectSummary, (summary) => {
  return summary?.name;
});

export const selectSummaryTrackers = createSelector(
  selectSummary,
  (summary) => {
    return summary?.trackers.map((tracker) => {
      return {
        ...tracker,
        trackerHistory: summary.players.reduce((acc, player) => {
          // Starting value
          acc.push({
            playerId: player.id,
            trackerId: tracker.id,
            newValue: tracker.startingValue,
            secondsSinceGameStart: 0,
          } as PlayerTrackerHistory);

          for (const th of player.trackerHistory.filter(
            (th) => th.trackerId === tracker.id,
          )) {
            acc.push({
              ...th,
              playerId: player.id,
            } as PlayerTrackerHistory);
          }
          return acc;
        }, new Array<PlayerTrackerHistory>()),
      } as TrackerSummary;
    });
  },
);

export const selectSummaryPlayers = createSelector(selectSummary, (summary) => {
  return (
    summary?.players.map((player) => {
      return {
        ...player,
        turnCount: player.turns.length,
        avgTurnDuration:
          player.turns
            .map((t) => t.durationSeconds)
            .reduce((acc, d) => acc + d, 0) / player.turns.length,
      } as PlayerSummary;
    }) || new Array<PlayerSummary>()
  );
});
