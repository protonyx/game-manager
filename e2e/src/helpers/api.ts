/**
 * API helpers for Game Manager endpoints
 * Provides functions to interact with backend API for game creation, joining, and state transitions
 */

import { customFetch } from './fetch-client';

export interface GameResponse {
  gameId: string;
  entryCode: string;
  createdByPlayerId: string;
  createdAt: string;
  [key: string]: any;
}

export interface JoinGameResponse {
  gameId: string;
  playerId: string;
  authToken: string;
  [key: string]: any;
}

export interface GameStartResponse {
  gameId: string;
  status: string;
  currentTurn: number;
  [key: string]: any;
}

/**
 * Create a new game on the backend
 * @param baseUrl Base URL of the API (e.g., http://localhost:5000)
 * @param gameName Name of the game to create
 * @returns Game response with gameId and entryCode
 */
export async function createGame(baseUrl: string, gameName: string): Promise<GameResponse> {
  const response = await customFetch(`${baseUrl}/api/v1/Games`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      gameName: gameName,
      trackers: [
        {
          trackerName: 'TestTracker',
          gameTrackerMax: 10,
        },
      ],
    }),
  });

  if (!response.ok) {
    throw new Error(`Failed to create game: ${response.status} ${response.statusText}`);
  }

  return response.json();
}

/**
 * Join an existing game using entry code
 * @param baseUrl Base URL of the API
 * @param gameId ID of the game to join
 * @param entryCode Entry code for the game
 * @param playerName Name of the player joining
 * @returns Join response with authToken
 */
export async function joinGame(
  baseUrl: string,
  gameId: string,
  entryCode: string,
  playerName: string,
): Promise<JoinGameResponse> {
  const response = await customFetch(`${baseUrl}/api/v1/Games/Join`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      gameId: gameId,
      entryCode: entryCode,
      playerName: playerName,
    }),
  });

  if (!response.ok) {
    throw new Error(`Failed to join game: ${response.status} ${response.statusText}`);
  }

  return response.json();
}

/**
 * Start an existing game
 * @param baseUrl Base URL of the API
 * @param gameId ID of the game to start
 * @param authToken Authentication token
 * @returns Game start response
 */
export async function startGame(baseUrl: string, gameId: string, authToken: string): Promise<GameStartResponse> {
  const response = await customFetch(`${baseUrl}/api/v1/Games/${gameId}/Start`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${authToken}`,
    },
  });

  if (!response.ok) {
    throw new Error(`Failed to start game: ${response.status} ${response.statusText}`);
  }

  return response.json();
}

/**
 * Get game details
 * @param baseUrl Base URL of the API
 * @param gameId ID of the game to fetch
 * @param authToken Authentication token
 * @returns Game details
 */
export async function getGame(baseUrl: string, gameId: string, authToken: string): Promise<any> {
  const response = await customFetch(`${baseUrl}/api/v1/Games/${gameId}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${authToken}`,
    },
  });

  if (!response.ok) {
    throw new Error(`Failed to get game: ${response.status} ${response.statusText}`);
  }

  return response.json();
}
