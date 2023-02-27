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
            return {...state, players: [...state.players, message.player]}
        }),
        on(GameHubActions.playerUpdated, (state, message) => {
            let filteredPlayers = state.players.filter(item => item.id !== message.player.id)
            return {...state, players: [...filteredPlayers, message.player]}
        }),
        on(GameHubActions.playerLeft, (state, message) => {
            let newPlayers = state.players.filter(item => item.id !== message.playerId)
            return {...state, players: newPlayers}
        }),
        on(GamesApiActions.joinedGame, (state, {credentials}) => {
            return {...state, credentials: credentials};
        }),
        on(GamesApiActions.retrievedGame, (state, {game}) => {
            return {...state, game: game};
        }),
        on(GamesApiActions.retrievedPlayers, (state, {players}) => {
            return {...state, players: players};
        })
    ),
});

export const {
    name, // feature name
    reducer, // feature reducer
    selectGameState, // feature selector
    selectCredentials,
    selectGame,
    selectPlayers
} = gameFeature;
