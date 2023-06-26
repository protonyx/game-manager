import { createFeature, createReducer, on } from '@ngrx/store';
import { initialState } from './layout.state';
import { LayoutActions } from './layout.actions';

const layoutFeatureKey = 'layout';

export const layoutFeature = createFeature({
  name: layoutFeatureKey,
  reducer: createReducer(
    initialState,
    on(LayoutActions.setTitle, (state, { title }) => {
      return { ...state, title: title };
    }),
    on(LayoutActions.setEntryCode, (state, { entryCode }) => {
      return { ...state, entryCode: entryCode };
    }),
    on(LayoutActions.resetLayout, (state) => {
      return { ...state, title: 'Game Manager', entryCode: '' };
    })
  ),
});

export const {
  name,
  reducer,
  selectLayoutState,
  selectTitle,
  selectEntryCode,
} = layoutFeature;
