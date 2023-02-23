import {createReducer, on} from "@ngrx/store";

import {GamesApiActions} from "./games.actions";
import {GameState} from "./game.state";

export const initialState: GameState = {
    credentials: null,
    game: null,
    players: []
};

export const gamesReducer = createReducer(
    initialState,
    on(GamesApiActions.joinedGame, (state, {credentials}) => {
        return {...state, credentials: credentials};
    }),
    on(GamesApiActions.retrievedGameState, (state, {game}) => {
        return {...state, game: game};
    }),
    on(GamesApiActions.retrievedPlayers, (state, {players}) => {
        return {...state, players: players};
    })
);
