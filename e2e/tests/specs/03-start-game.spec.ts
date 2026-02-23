import { test, expect } from '@playwright/test';
import { createGame, joinGame, startGame, getGame } from '../../src/helpers/api';
import { generateGameName, generatePlayerName } from '../../src/helpers/game';
import { config, validateConfig } from '../../src/config/env.config';

test.beforeAll(() => {
  validateConfig();
});

test.describe('Start Game Flow', () => {
  test('should start a game and transition state', async () => {
    // Arrange
    const gameName = generateGameName('SmokeSuite_StartGame');
    const creatorName = generatePlayerName('start-test', 0);
    const createResponse = await createGame(config.baseUrl, gameName);
    const { gameId, entryCode } = createResponse;
    const joinResponse = await joinGame(config.baseUrl, gameId, entryCode, creatorName);

    // Act
    const startResponse = await startGame(config.baseUrl, gameId, joinResponse.authToken);

    // Assert
    expect(startResponse).toBeDefined();
    expect(startResponse.gameId).toBe(gameId);
    expect(startResponse.status).toBeTruthy();
    console.log(`✓ Game started successfully with status: ${startResponse.status}`);
  });

  test('should have valid game state after starting', async () => {
    // Arrange
    const gameName = generateGameName('SmokeSuite_GameState');
    const creatorName = generatePlayerName('state-test', 0);
    const createResponse = await createGame(config.baseUrl, gameName);
    const { gameId, entryCode } = createResponse;
    const joinResponse = await joinGame(config.baseUrl, gameId, entryCode, creatorName);

    // Act
    await startGame(config.baseUrl, gameId, joinResponse.authToken);
    const gameDetails = await getGame(config.baseUrl, gameId, joinResponse.authToken);

    // Assert
    expect(gameDetails).toBeDefined();
    expect(gameDetails.gameId).toBe(gameId);
    expect(gameDetails.currentTurn).toBeDefined();
    console.log(`✓ Game state valid - Current turn: ${gameDetails.currentTurn}`);
  });

  test('should maintain player list after game starts', async () => {
    // Arrange
    const gameName = generateGameName('SmokeSuite_PlayerList');
    const playerCount = 2;
    const createResponse = await createGame(config.baseUrl, gameName);
    const { gameId, entryCode } = createResponse;

    // Add first player and start game
    const player1Name = generatePlayerName(gameId, 1);
    const join1 = await joinGame(config.baseUrl, gameId, entryCode, player1Name);

    // Add second player
    const player2Name = generatePlayerName(gameId, 2);
    await joinGame(config.baseUrl, gameId, entryCode, player2Name);

    // Act
    await startGame(config.baseUrl, gameId, join1.authToken);
    const gameDetails = await getGame(config.baseUrl, gameId, join1.authToken);

    // Assert
    expect(gameDetails.players).toBeDefined();
    expect(gameDetails.players.length).toBeGreaterThanOrEqual(playerCount);
    console.log(`✓ Player list maintained - Players: ${gameDetails.players.length}`);
  });

  test('should support game lifecycle: create -> join -> start', async () => {
    // Arrange & Act - Complete lifecycle
    const gameName = generateGameName('SmokeSuite_Lifecycle');
    const playerName = generatePlayerName('lifecycle', 1);

    // Step 1: Create
    const createResponse = await createGame(config.baseUrl, gameName);
    const { gameId, entryCode } = createResponse;
    expect(createResponse.gameId).toBeTruthy();

    // Step 2: Join
    const joinResponse = await joinGame(config.baseUrl, gameId, entryCode, playerName);
    expect(joinResponse.authToken).toBeTruthy();

    // Step 3: Start
    const startResponse = await startGame(config.baseUrl, gameId, joinResponse.authToken);
    expect(startResponse.status).toBeTruthy();

    // Step 4: Verify
    const finalState = await getGame(config.baseUrl, gameId, joinResponse.authToken);
    expect(finalState.gameId).toBe(gameId);

    // Assert
    console.log(`✓ Complete game lifecycle successful: Create -> Join -> Start`);
  });
});
