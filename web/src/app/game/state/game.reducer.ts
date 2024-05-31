import { createFeature, createReducer, on } from '@ngrx/store';

import {
  GameActions,
  GameHubActions,
  GamesApiActions,
  PlayersApiActions,
} from './game.actions';
import { GameState, initialState, playerAdapter } from './game.state';

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

export const reducer = gameFeature.reducer;
