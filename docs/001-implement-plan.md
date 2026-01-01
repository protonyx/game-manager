# Game Page UI Implementation Plan

This document outlines the detailed implementation plan for the UI improvements described in the [UI Improvement Plan](ui-plan.md).

## Files to be Updated

### 1. Game Page Component

**File:** `web/src/app/game/pages/game-page/game-page.component.html`
**Changes:**
- Replace the vertical accordion layout with a responsive grid layout
- Organize components into a two-column layout on larger screens
- Maintain a single-column layout on smaller screens

**File:** `web/src/app/game/pages/game-page/game-page.component.ts`
**Changes:**
- Add imports for MatGridListModule and other required modules
- Add BreakpointObserver for responsive layout
- Add methods to handle screen size changes

**File:** `web/src/app/game/pages/game-page/game-page.component.scss`
**Changes:**
- Add styles for the grid layout
- Add responsive styles for different screen sizes
- Update styles for the active turn indicator

### 2. Player List Component

**File:** `web/src/app/game/components/player-list/player-list.component.html`
**Changes:**
- Replace table layout with card-based layout
- Add search input for filtering players
- Add pagination for large player lists
- Implement horizontal scrollable container for smaller screens

**File:** `web/src/app/game/components/player-list/player-list.component.ts`
**Changes:**
- Add methods for filtering players
- Update data source to work with pagination
- Add imports for MatPaginatorModule, MatCardModule, etc.

**File:** `web/src/app/game/components/player-list/player-list.component.scss`
**Changes:**
- Add styles for player cards
- Add styles for the search input and pagination
- Add responsive styles for different screen sizes

### 3. Tracker Editor Component

**File:** `web/src/app/game/components/tracker-editor/tracker-editor.component.html`
**Changes:**
- Replace table layout with a more compact horizontal layout
- Use slider components for numeric values
- Implement collapsible sections for tracker categories (if applicable)

**File:** `web/src/app/game/components/tracker-editor/tracker-editor.component.ts`
**Changes:**
- Update methods to work with sliders
- Add imports for MatSliderModule
- Add logic for tracker categories (if applicable)

**File:** `web/src/app/game/components/tracker-editor/tracker-editor.component.scss`
**Changes:**
- Add styles for the new layout
- Add styles for sliders and buttons
- Add responsive styles for different screen sizes

### 4. Current Turn Component

**File:** `web/src/app/game/components/current-turn/current-turn.component.html`
**Changes:**
- Replace simple layout with a card-based layout
- Add visual indicators for turn progress
- Include more contextual information about the current player's status

**File:** `web/src/app/game/components/current-turn/current-turn.component.ts`
**Changes:**
- Add method to get player color
- Update imports to include MatCardModule
- Add any additional logic needed for new features

**File:** `web/src/app/game/components/current-turn/current-turn.component.scss`
**Changes:**
- Add styles for the card layout
- Add styles for turn progress indicators
- Add responsive styles for different screen sizes

### 5. Game Control Component

**File:** `web/src/app/game/components/game-control/game-control.component.html`
**Changes:**
- Replace action row with a toolbar layout
- Use icon buttons with tooltips
- Add a menu for less frequently used operations

**File:** `web/src/app/game/components/game-control/game-control.component.ts`
**Changes:**
- Update imports to include MatToolbarModule, MatTooltipModule, etc.
- Add any additional logic needed for new features

**File:** `web/src/app/game/components/game-control/game-control.component.scss`
**Changes:**
- Add styles for the toolbar layout
- Add styles for icon buttons and menu
- Add responsive styles for different screen sizes

## Implementation Steps

### Phase 1: Responsive Grid Layout

[x] Update the game page component to use a grid layout
   - Add MatGridListModule to imports
   - Add BreakpointObserver for responsive design
   - Update the template to use grid layout
   - Update styles for the grid layout

[x] Test the grid layout on different screen sizes
   - Ensure components are properly positioned
   - Ensure layout adapts to different screen sizes
   - Fix any issues with component positioning

### Phase 2: Component Redesigns

[x] Update the player list component
   - Replace table with card-based layout
   - Add search functionality
   - Add pagination
   - Test with various numbers of players

[x] Update the tracker editor component
   - Replace table with horizontal layout
   - Implement sliders for numeric values
   - Test with various types of trackers

[x] Update the current turn component
   - Implement card-based layout
   - Add visual indicators for turn progress
   - Test with different game states

[ ] Update the game control component
   - Implement toolbar layout with icon buttons
   - Add menu for less frequently used operations
   - Test with different game states

### Phase 3: Responsive Design and Polish

[ ] Ensure all components are responsive
   - Test on various screen sizes
   - Adjust layouts as needed
   - Ensure components work well together

[ ] Add animations and transitions
   - Add smooth transitions for expanding/collapsing sections
   - Add highlight effects for the current player
   - Add progress indicators for turn timers

[ ] Final testing and bug fixes
   - Test all components together
   - Test on various devices and screen sizes
   - Fix any remaining issues

## Technical Requirements

### New Angular Material Modules to Import

- MatGridListModule
- MatCardModule
- MatPaginatorModule
- MatSliderModule
- MatToolbarModule
- MatTooltipModule
- MatInputModule (for search)
- MatIconModule (for icons)
- MatBadgeModule (for badges)

### CSS Requirements

- Use CSS Grid and Flexbox for layouts
- Use Angular Material's breakpoint observer for responsive design
- Follow Material Design color system
- Implement proper elevation (shadows) for components
- Ensure consistent typography across components

## Conclusion

This implementation plan provides a detailed roadmap for updating the game page UI according to the improvement suggestions in the UI plan. By following these steps, the development team can systematically improve the user experience by reducing vertical scrolling, making better use of screen space, and organizing information more effectively while adhering to Material Design principles.
