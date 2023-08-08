import { createFeature, createReducer, createSelector, on } from '@ngrx/store';

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
  reducer: createReducer(
    initialState,
    on(GameHubActions.hubConnected, (state) => {
      return { ...state, hubConnected: true };
    }),
    on(GameHubActions.hubDisconnected, (state) => {
      return { ...state, hubConnected: false };
    }),
    on(GameHubActions.gameUpdated, (state, { game }) => {
      return { ...state, game: game };
    }),
    on(GameHubActions.playerJoined, (state, message) => {
      return {
        ...state,
        players: playerAdapter.addOne(message.player, state.players),
      };
    }),
    on(GameHubActions.playerUpdated, (state, message) => {
      return {
        ...state,
        players: playerAdapter.setOne(message.player, state.players),
      };
    }),
    on(GameHubActions.playerLeft, (state, message) => {
      return {
        ...state,
        players: playerAdapter.removeOne(message.playerId, state.players),
      };
    }),
    on(GameHubActions.credentialsUpdated, (state, { credentials }) => {
      return {
        ...state,
        credentials: credentials,
      };
    }),
    on(GamesApiActions.joinedGame, (state, { credentials }) => {
      return { ...state, credentials: credentials };
    }),
    on(GamesApiActions.retrievedGame, (state, { game }) => {
      return { ...state, game: game };
    }),
    on(GamesApiActions.retrievedGameSummary, (state, { summary }) => {
      return { ...state, summary: summary };
    }),
    on(GamesApiActions.retrievedPlayers, (state, { players }) => {
      return {
        ...state,
        players: playerAdapter.setAll(players, state.players),
      };
    }),
    on(GameActions.clearCredentials, (state) => {
      // Game or Player is no longer valid, reset game state
      return {
        ...state,
        currentPlayer: null,
        credentials: null,
        game: null,
        players: playerAdapter.removeAll(state.players),
      };
    }),
    on(GameActions.updateTracker, (state, { playerId, tracker }) => {
      return {
        ...state,
        players: playerAdapter.updateOne(
          {
            id: playerId,
            changes: { trackerValues: { [tracker.trackerId]: tracker.value } },
          },
          state.players
        ),
      };
    }),
    on(PlayersApiActions.playerRemoved, (state, { playerId }) => {
      return {
        ...state,
        players: playerAdapter.removeOne(playerId, state.players),
      };
    })
  ),
});

export const {
  name, // feature name
  reducer, // feature reducer
  selectGameState, // feature selector
  selectHubConnected,
  selectCredentials,
  selectGame,
  selectPlayers,
  selectSummary,
} = gameFeature;

const { selectIds, selectEntities, selectAll, selectTotal } =
  playerAdapter.getSelectors();

export const selectPlayerIds = createSelector(selectPlayers, selectIds);

export const selectPlayersEntities = createSelector(
  selectPlayers,
  selectEntities
);

export const selectAllPlayers = createSelector(selectPlayers, selectAll);

export const selectTotalPlayers = createSelector(selectPlayers, selectTotal);

export const selectCurrentPlayerId = createSelector(
  selectCredentials,
  (credentials) => credentials?.playerId
);

export const selectCurrentPlayer = createSelector(
  selectCurrentPlayerId,
  selectPlayersEntities,
  (playerId, entities) => (playerId ? entities[playerId] : null)!
);
