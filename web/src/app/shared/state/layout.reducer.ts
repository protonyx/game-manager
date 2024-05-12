import { createFeature, createReducer, on } from '@ngrx/store';
import { initialState, LayoutState } from './layout.state';
import { LayoutActions } from './layout.actions';

const layoutFeatureKey = 'layout';

export const layoutFeature = createFeature({
  name: layoutFeatureKey,
  reducer: createReducer<LayoutState>(
    initialState,
    on(LayoutActions.setTitle, (state, { title }): LayoutState => {
      return { ...state, title: title };
    }),
    on(LayoutActions.setEntryCode, (state, { entryCode }): LayoutState => {
      return { ...state, entryCode: entryCode };
    }),
    on(LayoutActions.setHeader, (state, { title, entryCode }): LayoutState => {
      return { ...state, title: title, entryCode: entryCode };
    }),
    on(LayoutActions.resetLayout, (state): LayoutState => {
      return {
        ...state,
        title: initialState.title,
        entryCode: initialState.entryCode,
      };
    }),
  ),
});

export const { name, reducer, selectTitle, selectEntryCode } = layoutFeature;
