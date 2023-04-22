import {createFeature, createReducer, on} from "@ngrx/store";

import {GameActions, GameHubActions, GamesApiActions, PlayersApiActions} from "./game.actions";
import {GameState, initialState} from "./game.state";
import {Player} from "../models/models";


export const gameFeature = createFeature({
    name: 'game',
    reducer: createReducer(
        initialState,
        on(GameHubActions.gameUpdated, (state, {game}) => {
            return {...state, game: game}
        }),
        on(GameHubActions.playerJoined, (state, message) => {
            let newPlayers = [...state.players, message.player].sort((a, b) => a.order - b.order);
            return {...state, players: newPlayers}
        }),
        on(GameHubActions.playerUpdated, (state, message) => {
            let filteredPlayers = state.players.filter(item => item.id !== message.player.id)
            let newPlayers = [...filteredPlayers, message.player].sort((a, b) => a.order - b.order);
            return {...state, players: newPlayers}
        }),
        on(GameHubActions.playerLeft, (state, message) => {
            let newPlayers = state.players.filter(item => item.id !== message.playerId)
            newPlayers.sort((a, b) => a.order - b.order);
            return {...state, players: newPlayers}
        }),
        on(GamesApiActions.joinedGame, (state, {credentials}) => {
            return {...state, credentials: credentials};
        }),
        on(GamesApiActions.retrievedGame, (state, {game}) => {
            return {...state, game: game};
        }),
        on(GamesApiActions.retrievedPlayers, (state, {players}) => {
            let newPlayers = [...players].sort((a, b) => a.order - b.order);
            return {...state, players: newPlayers};
        }),
        on(GamesApiActions.retrievedCurrentPlayer, (state, {player}) => {
            return {...state, currentPlayer: player};
        }),
        on(GameActions.clearCredentials, (state) => {
            // Game or Player is no longer valid, remove game state
            return {...state, currentPlayer: null, credentials: null, game: null, players: []};
        }),
        on(GameActions.updateTracker, (state, {tracker}) => {
            const player = {...state.currentPlayer, trackerValues: {
                ...state.currentPlayer?.trackerValues,
                    [tracker.trackerId]: tracker.value
                }
            } as Player;
            return {...state, currentPlayer: player};
        }),
        on(PlayersApiActions.playerRemoved, (state, {playerId}) => {
            if (state.currentPlayer?.id === playerId) {
                return {...state, currentPlayer: null, credentials: null, game: null, players: []};
            } else {
                return {...state};
            }
        })
    ),
});

export const {
    name, // feature name
    reducer, // feature reducer
    selectGameState, // feature selector
    selectCredentials,
    selectCurrentPlayer,
    selectGame,
    selectPlayers
} = gameFeature;
