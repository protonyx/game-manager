import { createActionGroup, emptyProps, props } from '@ngrx/store';

export const LayoutActions = createActionGroup({
  source: 'Layout',
  events: {
    'Set Title': props<{ title: string }>(),
    'Set Header': props<{ title: string; entryCode: string }>(),
    'Reset Layout': emptyProps(),
    'Open Sidenav': emptyProps(),
    'Close Sidenav': emptyProps(),
    'Toggle Sidenav': emptyProps(),
  },
});
