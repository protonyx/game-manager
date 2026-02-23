/**
 * Game utilities for test data generation
 * Provides functions to generate unique game names, player names, and other test data
 */

/**
 * Generate a unique game name with timestamp
 * @param prefix Optional prefix for the game name
 * @returns Unique game name
 */
export function generateGameName(prefix = 'TestGame'): string {
  const timestamp = Date.now();
  const random = Math.random().toString(36).substring(7);
  return `${prefix}_${timestamp}_${random}`;
}

/**
 * Generate a unique player name with counter
 * @param gameId Game ID to associate with player
 * @param playerNumber Number for the player (1, 2, 3, etc.)
 * @returns Unique player name
 */
export function generatePlayerName(gameId: string, playerNumber: number): string {
  return `Player_${gameId.substring(0, 5)}_${playerNumber}`;
}

/**
 * Parse entry code from game response or URL
 * Entry codes are typically alphanumeric strings
 * @param entryCode Raw entry code from API response
 * @returns Formatted entry code
 */
export function formatEntryCode(entryCode: string): string {
  return entryCode?.toUpperCase() || '';
}

/**
 * Validate entry code format
 * @param entryCode Entry code to validate
 * @returns true if entry code is valid
 */
export function isValidEntryCode(entryCode: string): boolean {
  return !!(entryCode && entryCode.length > 0 && /^[A-Z0-9]+$/.test(entryCode));
}

/**
 * Generate a timeout delay for retrying operations
 * @param attempt Number of attempts already made
 * @returns Delay in milliseconds (exponential backoff)
 */
export function getRetryDelay(attempt: number): number {
  return Math.min(1000 * Math.pow(2, attempt), 10000);
}
