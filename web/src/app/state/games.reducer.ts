import {createReducer, on} from "@ngrx/store";

import {GamesApiActions} from "./games.actions";
import {Game} from "../models/models";

export const initialState: any = {};

export const gamesReducer = createReducer(
    initialState,
    on(GamesApiActions.joinedGame, (state, {credentials}) => {
        return {...state, credentials: credentials};
    }),
    on(GamesApiActions.retrievedGameState, (state, {game}) => {
        return {...state, game: game};
    })
);
