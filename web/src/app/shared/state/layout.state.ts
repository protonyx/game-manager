export interface LayoutState {
  title: string;
  entryCode: string;
  sidenavOpen: boolean;
}

export const initialState: LayoutState = {
  title: 'Game Manager',
  entryCode: '',
  sidenavOpen: false
};
