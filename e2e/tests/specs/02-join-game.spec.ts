import { test, expect } from '@playwright/test';
import { createGame, joinGame, getGame } from '../../src/helpers/api';
import { generateGameName, generatePlayerName } from '../../src/helpers/game';
import { config, validateConfig } from '../../src/config/env.config';
import { isValidToken, decodeToken } from '../../src/helpers/auth';

test.beforeAll(() => {
  validateConfig();
});

test.describe('Join Game Flow', () => {
  test('should join a game with valid entry code', async () => {
    // Arrange
    const gameName = generateGameName('SmokeSuite_Join');
    const playerName = generatePlayerName('join-test', 1);
    const createResponse = await createGame(config.baseUrl, gameName);
    const { gameId, entryCode } = createResponse;

    // Act
    const joinResponse = await joinGame(config.baseUrl, gameId, entryCode, playerName);

    // Assert
    expect(joinResponse).toBeDefined();
    expect(joinResponse.gameId).toBe(gameId);
    expect(joinResponse.playerId).toBeTruthy();
    expect(joinResponse.authToken).toBeTruthy();

    console.log(`✓ Player joined successfully: PlayerId=${joinResponse.playerId}`);
  });

  test('should receive valid JWT token when joining', async () => {
    // Arrange
    const gameName = generateGameName('SmokeSuite_JWT');
    const playerName = generatePlayerName('jwt-test', 1);
    const createResponse = await createGame(config.baseUrl, gameName);
    const { gameId, entryCode } = createResponse;

    // Act
    const joinResponse = await joinGame(config.baseUrl, gameId, entryCode, playerName);

    // Assert
    expect(isValidToken(joinResponse.authToken)).toBe(true);

    const payload = decodeToken(joinResponse.authToken);
    expect(payload).toBeDefined();
    console.log(`✓ Valid JWT token received with claims:`, Object.keys(payload).join(', '));
  });

  test('should authenticate subsequent requests with token', async () => {
    // Arrange
    const gameName = generateGameName('SmokeSuite_Auth');
    const playerName = generatePlayerName('auth-test', 1);
    const createResponse = await createGame(config.baseUrl, gameName);
    const { gameId, entryCode } = createResponse;
    const joinResponse = await joinGame(config.baseUrl, gameId, entryCode, playerName);

    // Act
    const gameDetails = await getGame(config.baseUrl, gameId, joinResponse.authToken);

    // Assert
    expect(gameDetails).toBeDefined();
    expect(gameDetails.gameId).toBe(gameId);
    console.log(`✓ Successfully authenticated subsequent request with token`);
  });

  test('should allow multiple players to join same game', async () => {
    // Arrange
    const gameName = generateGameName('SmokeSuite_Multiplayer');
    const createResponse = await createGame(config.baseUrl, gameName);
    const { gameId, entryCode } = createResponse;

    // Act
    const player1 = generatePlayerName(gameId, 1);
    const player2 = generatePlayerName(gameId, 2);
    const join1 = await joinGame(config.baseUrl, gameId, entryCode, player1);
    const join2 = await joinGame(config.baseUrl, gameId, entryCode, player2);

    // Assert
    expect(join1.playerId).not.toBe(join2.playerId);
    expect(join1.authToken).not.toBe(join2.authToken);

    const playerList = await getGame(config.baseUrl, gameId, join1.authToken);
    expect(playerList.players).toBeDefined();
    expect(playerList.players.length).toBeGreaterThanOrEqual(2);
    console.log(`✓ Multiple players joined game successfully`);
  });
});
