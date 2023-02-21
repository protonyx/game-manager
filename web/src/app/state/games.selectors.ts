import { createSelector, createFeatureSelector } from '@ngrx/store';

export const selectGameFeature = createFeatureSelector<any>('game');

export const selectGame = createSelector(
    selectGameFeature,
    (game) => {
        return game.game;
    }
);

export const selectCredentials = createSelector(
    selectGameFeature,
    (game) => {
        return game.credentials;
    }
);
