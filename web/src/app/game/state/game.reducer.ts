import { createFeature, createReducer, createSelector, on } from '@ngrx/store';

import {
  GameActions,
  GameHubActions,
  GamesApiActions,
  PlayersApiActions,
} from './game.actions';
import { GameState, initialState, playerAdapter } from './game.state';
import {
  PlayerSummary,
  PlayerTrackerHistory,
  TrackerSummary,
} from '../models/models';

export const gameFeatureKey = 'game';

export const gameFeature = createFeature({
  name: gameFeatureKey,
  reducer: createReducer<GameState>(
    initialState,
    on(GameHubActions.hubConnected, (state): GameState => {
      return { ...state, hubConnected: true };
    }),
    on(GameHubActions.hubDisconnected, (state): GameState => {
      return { ...state, hubConnected: false };
    }),
    on(GameHubActions.gameUpdated, (state, { game }): GameState => {
      return { ...state, game: game };
    }),
    on(GameHubActions.playerJoined, (state, message): GameState => {
      return {
        ...state,
        players: playerAdapter.addOne(message.player, state.players),
      };
    }),
    on(GameHubActions.playerUpdated, (state, message): GameState => {
      return {
        ...state,
        players: playerAdapter.setOne(message.player, state.players),
      };
    }),
    on(GameHubActions.playerLeft, (state, message): GameState => {
      return {
        ...state,
        players: playerAdapter.removeOne(message.playerId, state.players),
      };
    }),
    on(
      GameHubActions.credentialsUpdated,
      (state, { credentials }): GameState => {
        return {
          ...state,
          credentials: credentials,
        };
      },
    ),
    on(GamesApiActions.joinedGame, (state, { credentials }): GameState => {
      return { ...state, credentials: credentials };
    }),
    on(GamesApiActions.retrievedGame, (state, { game }): GameState => {
      return { ...state, game: game };
    }),
    on(
      GamesApiActions.retrievedGameSummary,
      (state, { summary }): GameState => {
        return { ...state, summary: summary };
      },
    ),
    on(GamesApiActions.retrievedPlayers, (state, { players }): GameState => {
      return {
        ...state,
        players: playerAdapter.setAll(players, state.players),
      };
    }),
    on(GameActions.clearCredentials, (state): GameState => {
      // Game or Player is no longer valid, reset game state
      return {
        ...state,
        credentials: null,
        game: null,
        players: playerAdapter.removeAll(state.players),
      };
    }),
    on(GameActions.updateTracker, (state, { playerId, tracker }): GameState => {
      return {
        ...state,
        players: playerAdapter.updateOne(
          {
            id: playerId,
            changes: { trackerValues: { [tracker.trackerId]: tracker.value } },
          },
          state.players,
        ),
      };
    }),
    on(PlayersApiActions.playerRemoved, (state, { playerId }): GameState => {
      return {
        ...state,
        players: playerAdapter.removeOne(playerId, state.players),
      };
    }),
  ),
});

export const {
  name, // feature name
  reducer, // feature reducer
  selectHubConnected,
  selectCredentials,
  selectGame,
  selectPlayers,
  selectSummary,
} = gameFeature;

export const selectGameTrackers = createSelector(
  selectGame,
  (game) => game?.trackers
);

const { selectEntities, selectAll } = playerAdapter.getSelectors();

export const selectPlayersEntities = createSelector(
  selectPlayers,
  selectEntities,
);

export const selectAllPlayers = createSelector(selectPlayers, selectAll);

export const selectPlayerById = (playerId: string) => createSelector(
  selectPlayersEntities,
  (playerEntities) => playerId && playerEntities[playerId],
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
  (playerId, entities) => (playerId ? entities[playerId] : null)!,
);

export const selectSummaryName = createSelector(selectSummary, (summary) => {
  return summary?.name;
});

export const selectSummaryTrackers = createSelector(
  selectSummary,
  (summary) => {
    return summary?.trackers.map((tracker) => {
      return <TrackerSummary>{
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
      };
    });
  },
);

export const selectSummaryPlayers = createSelector(selectSummary, (summary) => {
  return (
    summary?.players.map((player) => {
      return <PlayerSummary>{
        ...player,
        turnCount: player.turns.length,
        avgTurnDuration:
          player.turns
            .map((t) => t.durationSeconds)
            .reduce((acc, d) => acc + d, 0) / player.turns.length,
      };
    }) || new Array<PlayerSummary>()
  );
});
