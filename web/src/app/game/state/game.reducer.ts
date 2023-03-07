import {createFeature, createReducer, on} from "@ngrx/store";

import {GameActions, GameHubActions, GamesApiActions} from "./game.actions";
import {GameState, initialState} from "./game.state";


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
